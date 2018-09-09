using System;
using OpenTracing.Propagation;
using static OpenTracing.ApplicationInsights.Internal.NoOpHelpers;

namespace OpenTracing.ApplicationInsights
{
    /// <summary>
    /// OpenTracing <see cref="ITracer"/> implementation built specifically for use with Microsoft Application Insights.
    /// </summary>
    /// <remarks>
    /// Follows the guidelines for adapting the Application Insights data model to OpenTracing here: 
    /// https://docs.microsoft.com/en-us/azure/application-insights/application-insights-correlation#open-tracing-and-application-insights
    /// </remarks>
    public sealed class ApplicationInsightsTracer : ITracer
    {
        public ISpanBuilder BuildSpan(string operationName)
        {
            throw new NotImplementedException();
        }

        public void Inject<TCarrier>(ISpanContext spanContext, IFormat<TCarrier> format, TCarrier carrier)
        {
            throw new NotImplementedException();
        }

        public ISpanContext Extract<TCarrier>(IFormat<TCarrier> format, TCarrier carrier)
        {
            throw new NotImplementedException();
        }

        public IScopeManager ScopeManager { get; }
        public ISpan ActiveSpan => ScopeManager.Active?.Span ?? NoOpSpan;
    }
}
