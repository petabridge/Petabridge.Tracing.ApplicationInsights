// -----------------------------------------------------------------------
// <copyright file="B3Propagation.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using OpenTracing.Propagation;

namespace OpenTracing.ApplicationInsights.Propagation
{
    /// <inheritdoc />
    /// <summary>
    ///     Propagation system using B3 Headers supported by Zipkin and other tracers.
    /// </summary>
    /// <remarks>
    ///     See https://github.com/openzipkin/b3-propagation for implementation details.
    ///     Went with this option because the propagation settings described by Microsoft
    ///     are more complicated:
    ///     https://github.com/dotnet/corefx/blob/master/src/System.Diagnostics.DiagnosticSource/src/HttpCorrelationProtocol.md
    /// </remarks>
    public sealed class B3Propagator : IPropagator<ITextMap>
    {
        internal const string B3TraceId = "X-B3-TraceId";
        internal const string B3SpanId = "X-B3-SpanId";
        internal const string B3ParentId = "X-B3-ParentSpanId";

        public void Inject(ApplicationInsightsSpanContext context, ITextMap carrier)
        {
            carrier.Set(B3TraceId, context.TraceId);
            carrier.Set(B3SpanId, context.SpanId);
            if (context.ParentId != null)
                carrier.Set(B3ParentId, context.ParentId);
        }

        public ApplicationInsightsSpanContext Extract(ITextMap carrier)
        {
            string traceId = null;
            string spanId = null;
            string parentId = null;
            foreach (var entry in carrier)
                switch (entry.Key)
                {
                    case B3TraceId:
                        traceId = entry.Value;
                        break;
                    case B3SpanId:
                        spanId = entry.Value;
                        break;
                    case B3ParentId:
                        parentId = entry.Value;
                        break;
                }

            if (traceId != null && spanId != null) // don't care of ParentId is null or not
                return new ApplicationInsightsSpanContext(traceId, spanId, parentId);
            return null;
        }
    }
}