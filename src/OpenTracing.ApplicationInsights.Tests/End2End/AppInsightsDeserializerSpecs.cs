// -----------------------------------------------------------------------
// <copyright file="AppInsightsDeserializerSpecs.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System.IO;
using FluentAssertions;
using Xunit;

namespace OpenTracing.ApplicationInsights.Tests.End2End
{
    /// <summary>
    ///     Sanity checks for ensuring that <see cref="AppInsightsDeserializer" />
    ///     works as expected.
    /// </summary>
    public class AppInsightsDeserializerSpecs
    {
        public AppInsightsDeserializerSpecs()
        {
            _sampleJson = File.ReadAllText("End2End/appInsightsResponse.json");
        }

        private readonly string _sampleJson;

        [Fact(DisplayName = "Should be able to deserialize AppInsights REST JSON")]
        public void ShouldDeserializeValidAppInsightsJson()
        {
            var obj = AppInsightsDeserializer.DeserializeResult(_sampleJson);
            obj.tables.Count.Should().Be(1);
            obj.tables[0].rows.Count.Should().Be(4); // 4 ops in this JSON
        }
    }
}