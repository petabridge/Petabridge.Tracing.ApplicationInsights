using System;
using FluentAssertions;
using OpenTracing.ApplicationInsights.Util;
using OpenTracing.Noop;
using Xunit;
using static OpenTracing.ApplicationInsights.Util.ThreadLocalRngIdProvider;

namespace OpenTracing.ApplicationInsights.Tests
{
    public class SpanContextSpecs
    {
        [Fact(DisplayName = "root SpanContext should equal itself")]
        public void RootSpanContextShouldEqualSelf()
        {
            var context = new ApplicationInsightsSpanContext(NextId(), NextId());
            context.Equals(context).Should().BeTrue();
        }

        [Fact(DisplayName = "child SpanContext should equal itself")]
        public void ChildSpanContextShouldEqualSelf()
        {
            var context = new ApplicationInsightsSpanContext(NextId(), NextId(), NextId());
            context.Equals(context).Should().BeTrue();
        }

        [Fact(DisplayName = "NoOpSpanContext should be empty")]
        public void NoOpSpanContextShouldBeEmpty()
        {
            var context = NoOpHelpers.NoOpSpan.Context;
            context.IsEmpty().Should().BeTrue();
            context.IsAppInsightsSpan().Should().BeFalse();
        }
    }
}
