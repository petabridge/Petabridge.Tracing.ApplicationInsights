﻿// -----------------------------------------------------------------------
// <copyright file="AppInsightsRequestEnd2EndSpec.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

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

        [Fact(DisplayName = "RequestSpans: should be able to push simple RequestSpan to AppInsights")]
        public async Task ShouldPushStandAloneSpanToAppInsights()
        {
            string traceId = null;
            using (var span = _tracer.BuildSpan("simple1").StartActive())
            {
                traceId = span.Span.Context.TraceId;

                // sanity check
                traceId.Should().NotBeNullOrEmpty();
                span.Span.Context.SpanId.Should().NotBeNullOrEmpty();
            }

            // give the span a chance to be uploaded and processed
            await Task.Delay(500);

            var queryResult = await _fixture.QueryOperationsForTraceId(traceId);
            queryResult.isSuccess.Should().BeTrue();
        }
    }
}