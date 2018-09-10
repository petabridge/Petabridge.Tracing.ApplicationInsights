using System;
using System.Collections.Generic;
using System.Text;
using static OpenTracing.ApplicationInsights.Util.NoOpHelpers;

namespace OpenTracing.ApplicationInsights.Util
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
            return context == null || NoOpSpan.Context.Equals(context);
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
