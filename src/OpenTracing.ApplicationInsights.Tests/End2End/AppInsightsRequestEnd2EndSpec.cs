// -----------------------------------------------------------------------
// <copyright file="AppInsightsRequestEnd2EndSpec.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using static OpenTracing.ApplicationInsights.Tests.End2End.ExponentialBackoff;

namespace OpenTracing.ApplicationInsights.Tests.End2End
{
    public class AppInsightsRequestEnd2EndSpec : IClassFixture<AppInsightsFixture>
    {
        public AppInsightsRequestEnd2EndSpec(AppInsightsFixture fixture)
        {
            _fixture = fixture;
            _tracer = new ApplicationInsightsTracer(fixture.TelemetryConfiguration);
        }

        private readonly AppInsightsFixture _fixture;
        private readonly ApplicationInsightsTracer _tracer;

        [Fact(DisplayName = "RequestSpans: should be able to push parent and child RequestSpans to AppInsights")]
        public async Task ShouldPushParentAndChildRequestSpansToAppInsights()
        {
            if (!_fixture.EnableIntegrationSpecs)
                return;

            string traceId = null;
            using (var span = _tracer.BuildSpan("op1").StartActive())
            {
                traceId = span.Span.Context.TraceId;

                using (var child = _tracer.BuildSpan("op1.child").StartActive())
                {
                    child.Span.SetTag("child", true);

                    using (var grandChild = _tracer.BuildSpan("op1.grandchild").StartActive())
                    {
                        grandChild.Span.SetTag("child", true).SetTag("grandChild", true);
                    }
                }
            }

            // give the span a chance to be uploaded and processed
            await AwaitAssert(async () =>
            {
                var queryResult = await _fixture.QueryOperationsForTraceId(traceId);
                queryResult.isSuccess.Should().BeTrue();

                var parsedResults = AppInsightsDeserializer.DeserializeResult(queryResult.response);
                parsedResults.tables.Count.Should().Be(1);
                parsedResults.tables[0].rows.Count.Should().Be(3); // parent, child, and grandchild

                // TODO: deserialize table rows and validate parent --> child --> grandchild relationships
                // So far we've manually verified this through AppInsights dashboard and analytics queries
            });
        }

        [Fact(DisplayName = "RequestSpans: should be able to push span and correlated logs to AppInsights")]
        public async Task ShouldPushSpanLogstoAppInsights()
        {
            if (!_fixture.EnableIntegrationSpecs)
                return;

            string traceId = null;
            using (var span = _tracer.BuildSpan("op1").StartActive())
            {
                traceId = span.Span.Context.TraceId;

                // generate 3 trace events in AppInsights
                span.Span.Log("one").Log("two").Log("three");
            }

            // give the span a chance to be uploaded and processed
            await AwaitAssert(async () =>
            {
                var queryResult = await _fixture.QueryOperationsForTraceId(traceId);
                queryResult.isSuccess.Should().BeTrue();

                var parsedResults = AppInsightsDeserializer.DeserializeResult(queryResult.response);
                parsedResults.tables.Count.Should().Be(1);
                parsedResults.tables[0].rows.Count.Should().Be(4); // Request + 3 correlated traces
            });
        }

        [Fact(DisplayName = "RequestSpans: should be able to push simple RequestSpan to AppInsights")]
        public async Task ShouldPushStandAloneSpanToAppInsights()
        {
            if (!_fixture.EnableIntegrationSpecs)
                return;

            string traceId = null;
            using (var span = _tracer.BuildSpan("simple1").StartActive())
            {
                traceId = span.Span.Context.TraceId;

                // sanity check
                traceId.Should().NotBeNullOrEmpty();
                span.Span.Context.SpanId.Should().NotBeNullOrEmpty();
            }

            // give the span a chance to be uploaded and processed
            await AwaitAssert(async () =>
            {
                var queryResult = await _fixture.QueryOperationsForTraceId(traceId);
                queryResult.isSuccess.Should().BeTrue();

                var parsedResults = AppInsightsDeserializer.DeserializeResult(queryResult.response);
                parsedResults.tables.Count.Should().Be(1);
                parsedResults.tables[0].rows.Count.Should().Be(1);
            });
        }
    }
}