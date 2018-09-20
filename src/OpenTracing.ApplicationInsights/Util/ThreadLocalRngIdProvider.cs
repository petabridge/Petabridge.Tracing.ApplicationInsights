// -----------------------------------------------------------------------
// <copyright file="ThreadLocalRngIdProvider.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Threading;

namespace OpenTracing.ApplicationInsights.Util
{
    /// <summary>
    ///     INTERNAL API.
    ///     Used to generate randomized Trace and Operation IDs.
    /// </summary>
    internal static class ThreadLocalRngIdProvider
    {
        private static readonly ThreadLocal<Random> Rng = new ThreadLocal<Random>(() => new Random(), false);
        private static readonly ThreadLocal<byte[]> Buffers = new ThreadLocal<byte[]>(() => new byte[8], false);

        /// <summary>
        ///     Generates a random 128-bit identifier.
        /// </summary>
        /// <returns>A new string containing a hex representation of a 128 bit identifier.</returns>
        public static string NextId()
        {
            var id1 = NextIdLong();
            var id2 = NextIdLong();

            return string.Concat(id1.ToString("x16", CultureInfo.InvariantCulture),
                id2.ToString("x16", CultureInfo.InvariantCulture));
        }

        private static long NextIdLong()
        {
            Rng.Value.NextBytes(Buffers.Value);
            return BitConverter.ToInt64(Buffers.Value, 0);
        }
    }
}