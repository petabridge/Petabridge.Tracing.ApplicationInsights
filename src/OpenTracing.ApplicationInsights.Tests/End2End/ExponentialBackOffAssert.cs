// -----------------------------------------------------------------------
// <copyright file="ExponentialBackOffAssert.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenTracing.ApplicationInsights.Tests.End2End
{
    /// <summary>
    ///     Helper class for making life easier with <see cref="ExponentialBackOffAssert" />
    /// </summary>
    public static class ExponentialBackoff
    {
        /// <summary>
        ///     An expontential backoff table that will take 3 minutes, 43 seconds to execute.
        /// </summary>
        public static readonly IReadOnlyDictionary<int, TimeSpan> SafeDefaultBackoffSchedule =
            new Dictionary<int, TimeSpan>
            {
                {0, TimeSpan.FromMilliseconds(500)},
                {1, TimeSpan.FromMilliseconds(2500)},
                {2, TimeSpan.FromMilliseconds(10000)},
                {3, TimeSpan.FromMilliseconds(30000)},
                {4, TimeSpan.FromMilliseconds(60000)},
                {5, TimeSpan.FromMilliseconds(120000)}
            };

        public static async Task AwaitAssert(Func<Task> assertion)
        {
            var backoffAssert = new ExponentialBackOffAssert(SafeDefaultBackoffSchedule);
            await backoffAssert.Assert(assertion);
        }
    }

    /// <summary>
    ///     Given that the AppInsights API is.... very slow to record and process data that is uploaded right away.
    /// </summary>
    public class ExponentialBackOffAssert
    {
        public ExponentialBackOffAssert(int maxRetries, TimeSpan initialDelay)
            : this(ComputeBackoffs(maxRetries, initialDelay))
        {
        }

        public ExponentialBackOffAssert(IReadOnlyDictionary<int, TimeSpan> backoffTable)
        {
            BackoffTable = backoffTable;
        }

        public IReadOnlyDictionary<int, TimeSpan> BackoffTable { get; }

        public static IReadOnlyDictionary<int, TimeSpan> ComputeBackoffs(int maxRetries, TimeSpan initialDelay)
        {
            var seedMilliseconds = initialDelay.TotalMilliseconds;
            var backoffs = new Dictionary<int, TimeSpan>();
            backoffs[0] = initialDelay;
            for (var i = 1; i < maxRetries; i++)
                backoffs[i] = TimeSpan.FromMilliseconds(seedMilliseconds + seedMilliseconds * i);

            return backoffs;
        }

        public async Task Assert(Func<Task> assert, bool skipInitialDelay = false)
        {
            var i = 0;


            while (i < BackoffTable.Count)
                try
                {
                    if (i == 0 && skipInitialDelay)
                    {
                        i++;
                    }
                    else
                    {
                        await Task.Delay(BackoffTable[i]);
                        i++;
                    }

                    await assert();
                }
                catch (Exception ex)
                {
                    if (i < BackoffTable.Count)
                        Console.WriteLine("Exception: {0} - [{1}] tries remaining. Next one to commence in [{2}]",
                            ex.Message, BackoffTable.Count - i, BackoffTable[i]);
                    else
                        throw;
                }
        }
    }
}