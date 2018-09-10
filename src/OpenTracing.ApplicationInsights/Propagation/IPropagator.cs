using System;
using System.Collections.Generic;
using System.Text;

namespace OpenTracing.ApplicationInsights.Propagation
{
    /// <summary>
    ///     Used to inject and extract <see cref="ApplicationInsightsSpanContext" /> instances based on supported,
    ///     serialized formats.
    /// </summary>
    /// <typeparam name="TCarrier">The carrier format for this span context.</typeparam>
    public interface IPropagator<in TCarrier>
    {
        /// <summary>
        ///     Injects the <see cref="ApplicationInsightsSpanContext" /> into the carrier format.
        /// </summary>
        /// <param name="context">The underlying span's context.</param>
        /// <param name="carrier">The carrier format, used for transmission over the wire.</param>
        void Inject(ApplicationInsightsSpanContext context, TCarrier carrier);

        /// <summary>
        ///     Extracts the <see cref="ApplicationInsightsSpanContext" /> from the carrier format.
        /// </summary>
        /// <param name="carrier"></param>
        /// <returns></returns>
        ApplicationInsightsSpanContext Extract(TCarrier carrier);
    }
}
