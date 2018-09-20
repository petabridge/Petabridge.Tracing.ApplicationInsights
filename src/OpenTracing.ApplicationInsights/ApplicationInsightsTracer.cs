// -----------------------------------------------------------------------
// <copyright file="ApplicationInsightsTracer.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using OpenTracing.ApplicationInsights.Propagation;
using OpenTracing.Propagation;
using static OpenTracing.ApplicationInsights.Util.NoOpHelpers;

namespace OpenTracing.ApplicationInsights
{
    /// <summary>
    ///     OpenTracing <see cref="ITracer" /> implementation built specifically for use with Microsoft Application Insights.
    /// </summary>
    /// <remarks>
    ///     Follows the guidelines for adapting the Application Insights data model to OpenTracing here:
    ///     https://docs.microsoft.com/en-us/azure/application-insights/application-insights-correlation#open-tracing-and-application-insights
    /// </remarks>
    public sealed class ApplicationInsightsTracer : ITracer
    {
        private readonly TelemetryConfiguration _config;
        private readonly IPropagator<ITextMap> _propagator;

        public ApplicationInsightsTracer(TelemetryConfiguration config, IScopeManager scopeManager,
            IPropagator<ITextMap> propagator, ITimeProvider timeProvider, Endpoint localEndpoint)
        {
            Client = new TelemetryClient(config);
            _config = config;
            ScopeManager = scopeManager;
            _propagator = propagator;
            TimeProvider = timeProvider;
            LocalEndpoint = localEndpoint;
        }

        public ITimeProvider TimeProvider { get; }

        public TelemetryClient Client { get; }

        /// <summary>
        ///     The local endpoint for the node recording traces
        /// </summary>
        public Endpoint LocalEndpoint { get; }

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
    }
}