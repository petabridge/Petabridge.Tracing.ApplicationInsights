// -----------------------------------------------------------------------
// <copyright file="SpanExtensions.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using OpenTracing;

namespace Petabridge.Tracing.ApplicationInsights.Util
{
    /// <summary>
    ///     Extension methods for working with <see cref="ISpanContext" />
    /// </summary>
    public static class SpanExtensions
    {
        /// <summary>
        ///     Determines if this <see cref="ISpanContext" /> is empty or not.
        /// </summary>
        /// <param name="context">The span context.</param>
        /// <returns><c>true</c> if the span is empty, <c>false</c> otherwise.</returns>
        public static bool IsEmpty(this ISpanContext context)
        {
            return context == null || NoOpHelpers.NoOpSpan.Context.Equals(context);
        }

        /// <summary>
        ///     Determines if an <see cref="ISpanContext" /> is a valid <see cref="IApplicationInsightsSpanContext" /> or not.
        /// </summary>
        /// <param name="context">The span context.</param>
        /// <returns><c>true</c> if the span is valid, <c>false</c> otherwise.</returns>
        public static bool IsAppInsightsSpan(this ISpanContext context)
        {
            return !IsEmpty(context) && context is IApplicationInsightsSpanContext;
        }
    }
}