// -----------------------------------------------------------------------
// <copyright file="B3PropagatorSpecs.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using FluentAssertions;
using Microsoft.ApplicationInsights.Extensibility;
using OpenTracing.Propagation;
using Xunit;
using static OpenTracing.ApplicationInsights.Util.ThreadLocalRngIdProvider;

namespace OpenTracing.ApplicationInsights.Tests.Propagation
{
    public class B3PropagatorSpecs
    {
        public B3PropagatorSpecs()
        {
            // use whatever the active TelemetryConfiguration is, if any (probably none)
            Tracer = new ApplicationInsightsTracer(TelemetryConfiguration.Active);
        }

        public readonly IApplicationInsightsTracer Tracer;

        [Fact(DisplayName = "Should be able to extract and inject spans via B3 headers")]
        public void ShouldExtractAndInjectSpansViaB3()
        {
            var context = new ApplicationInsightsSpanContext(NextId(), NextId(), NextId());
            var carrier = new Dictionary<string, string>();

            Tracer.Inject(context, BuiltinFormats.HttpHeaders, new TextMapInjectAdapter(carrier));
            var extracted =
                (ApplicationInsightsSpanContext) Tracer.Extract(BuiltinFormats.HttpHeaders,
                    new TextMapExtractAdapter(carrier));

            context.Should().BeEquivalentTo(extracted);
        }

        [Fact(DisplayName = "Should be able to extract and inject spans withour ParentIds via B3 headers")]
        public void ShouldExtractAndInjectSpansWithoutParentsViaB3()
        {
            var context = new ApplicationInsightsSpanContext(NextId(), NextId());
            var carrier = new Dictionary<string, string>();

            Tracer.Inject(context, BuiltinFormats.HttpHeaders, new TextMapInjectAdapter(carrier));
            var extracted =
                (ApplicationInsightsSpanContext) Tracer.Extract(BuiltinFormats.HttpHeaders,
                    new TextMapExtractAdapter(carrier));

            context.Should().BeEquivalentTo(extracted);
            extracted.ParentId.Should().BeNullOrEmpty();
        }

        [Fact(DisplayName = "B3Propagator should not extract SpanContext when none found")]
        public void ShouldNotExtractAnyTraceIdWhenNoneFound()
        {
            // pass in an empty carrier
            var carrier = new Dictionary<string, string>();
            var extracted =
                (ApplicationInsightsSpanContext) Tracer.Extract(BuiltinFormats.HttpHeaders,
                    new TextMapExtractAdapter(carrier));

            extracted.Should().BeNull();
        }
    }
}