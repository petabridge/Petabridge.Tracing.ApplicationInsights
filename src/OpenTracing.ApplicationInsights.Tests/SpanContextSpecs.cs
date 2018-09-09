using System;
using FluentAssertions;
using OpenTracing.ApplicationInsights.Internal;
using Xunit;
using static OpenTracing.ApplicationInsights.Internal.ThreadLocalRngIdProvider;

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
    }
}
