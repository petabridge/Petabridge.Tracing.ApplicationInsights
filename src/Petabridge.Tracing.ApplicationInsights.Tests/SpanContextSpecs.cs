// -----------------------------------------------------------------------
// <copyright file="SpanContextSpecs.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Petabridge.Tracing.ApplicationInsights.Util;
using Xunit;

namespace Petabridge.Tracing.ApplicationInsights.Tests
{
    public class SpanContextSpecs
    {
        [Fact(DisplayName = "child SpanContext should equal itself")]
        public void ChildSpanContextShouldEqualSelf()
        {
            var context = new ApplicationInsightsSpanContext(ThreadLocalRngIdProvider.NextId(), ThreadLocalRngIdProvider.NextId(), ThreadLocalRngIdProvider.NextId());
            context.Equals(context).Should().BeTrue();
        }

        [Fact(DisplayName = "NoOpSpanContext should be empty")]
        public void NoOpSpanContextShouldBeEmpty()
        {
            var context = NoOpHelpers.NoOpSpan.Context;
            context.IsEmpty().Should().BeTrue();
            context.IsAppInsightsSpan().Should().BeFalse();
        }

        [Fact(DisplayName = "root SpanContext should equal itself")]
        public void RootSpanContextShouldEqualSelf()
        {
            var context = new ApplicationInsightsSpanContext(ThreadLocalRngIdProvider.NextId(), ThreadLocalRngIdProvider.NextId());
            context.Equals(context).Should().BeTrue();
        }
    }
}