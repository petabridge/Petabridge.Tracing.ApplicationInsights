using System;
using System.Collections.Generic;
using System.Text;
using OpenTracing.Noop;

namespace OpenTracing.ApplicationInsights.Internal
{
    /// <summary>
    /// INTERNAL API
    /// </summary>
    internal static class NoOpHelpers
    {
        /// <summary>
        /// Singleton NoOp span instance.
        /// </summary>
        public static readonly ISpan NoOpSpan = NoopTracerFactory.Create().BuildSpan("noop").Start();
    }
}
