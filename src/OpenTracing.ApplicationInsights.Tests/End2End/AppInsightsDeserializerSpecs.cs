using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using Xunit;

namespace OpenTracing.ApplicationInsights.Tests.End2End
{
    /// <summary>
    /// Sanity checks for ensuring that <see cref="AppInsightsDeserializer"/>
    /// works as expected.
    /// </summary>
    public class AppInsightsDeserializerSpecs
    {
        private readonly string _sampleJson;

        public AppInsightsDeserializerSpecs()
        {
            _sampleJson = File.ReadAllText("End2End/appInsightsResponse.json");
        }

        [Fact(DisplayName = "Should be able to deserialize AppInsights REST JSON")]
        public void ShouldDeserializeValidAppInsightsJson()
        {
            var obj = AppInsightsDeserializer.DeserializeResult(_sampleJson);
            obj.tables.Count.Should().Be(1);
            obj.tables[0].rows.Count.Should().Be(4); // 4 ops in this JSON
        }
    }
}
