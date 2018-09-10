using OpenTracing.Noop;

namespace OpenTracing.ApplicationInsights.Util
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
