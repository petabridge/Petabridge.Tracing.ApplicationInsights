using System;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using OpenTracing.ApplicationInsights.Propagation;
using OpenTracing.Propagation;
using static OpenTracing.ApplicationInsights.Util.NoOpHelpers;

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
        private readonly TelemetryConfiguration _config;
        private readonly IPropagator<ITextMap> _propagator;

        public ApplicationInsightsTracer(TelemetryConfiguration config, IScopeManager scopeManager, IPropagator<ITextMap> propagator, ITimeProvider timeProvider)
        {
            Client = new TelemetryClient(config);
            _config = config;
            ScopeManager = scopeManager;
            _propagator = propagator;
            TimeProvider = timeProvider;
        }

        public ITimeProvider TimeProvider { get; }

        public TelemetryClient Client { get; }

        public ISpanBuilder BuildSpan(string operationName)
        {
            return new ApplicationInsightsSpanBuilder(this, operationName);
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

        internal void Report(IApplicationInsightsSpan span)
        {
            ITelemetry telemetry = null;
            switch (span.SpanKind)
            {
                case SpanKind.CLIENT:
                    telemetry = new DependencyTelemetry()
                    {
                        Id = span.TypedContext.SpanId,
                        Duration = span.Duration ?? TimeSpan.Zero,
                        Name = span.OperationName,
                    };

                    break;
            }

            Client.Track(telemetry);
        }
    }
}
