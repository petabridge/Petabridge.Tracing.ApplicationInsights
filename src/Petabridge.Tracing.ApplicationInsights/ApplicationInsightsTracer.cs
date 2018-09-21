// -----------------------------------------------------------------------
// <copyright file="ApplicationInsightsTracer.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using OpenTracing;
using OpenTracing.Propagation;
using OpenTracing.Util;
using Petabridge.Tracing.ApplicationInsights.Propagation;
using Petabridge.Tracing.ApplicationInsights.Util;

namespace Petabridge.Tracing.ApplicationInsights
{
    /// <summary>
    ///     Interface describing behavior of the Application Insights <see cref="ITracer" /> implementation.
    /// </summary>
    public interface IApplicationInsightsTracer : ITracer
    {
        ITimeProvider TimeProvider { get; }
        TelemetryClient Client { get; }

        /// <summary>
        ///     The local endpoint for the node recording traces
        /// </summary>
        Endpoint LocalEndpoint { get; }

        /// <summary>
        ///     Hides the <see cref="ITracer.BuildSpan" /> operation so we can
        ///     return a typed <see cref="IApplicationInsightsSpanBuilder" />.
        /// </summary>
        /// <param name="operationName">The name of the current operation.</param>
        /// <returns>A new <see cref="IApplicationInsightsSpanBuilder" /> instance.</returns>
        new IApplicationInsightsSpanBuilder BuildSpan(string operationName);
    }

    /// <summary>
    ///     OpenTracing <see cref="ITracer" /> implementation built specifically for use with Microsoft Application Insights.
    /// </summary>
    /// <remarks>
    ///     Follows the guidelines for adapting the Application Insights data model to OpenTracing here:
    ///     https://docs.microsoft.com/en-us/azure/application-insights/application-insights-correlation#open-tracing-and-application-insights
    /// </remarks>
    public sealed class ApplicationInsightsTracer : IApplicationInsightsTracer
    {
        private readonly TelemetryConfiguration _config;
        private readonly IPropagator<ITextMap> _propagator;

        public ApplicationInsightsTracer(TelemetryConfiguration config, Endpoint localEndpoint = null) : this(config,
            new AsyncLocalScopeManager(),
            new B3Propagator(), new DateTimeOffsetTimeProvider(), localEndpoint)
        {
        }

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

        public Endpoint LocalEndpoint { get; }

        public IApplicationInsightsSpanBuilder BuildSpan(string operationName)
        {
            return new ApplicationInsightsSpanBuilder(this, operationName);
        }

        ISpanBuilder ITracer.BuildSpan(string operationName)
        {
            return BuildSpan(operationName);
        }

        public void Inject<TCarrier>(ISpanContext spanContext, IFormat<TCarrier> format, TCarrier carrier)
        {
            if ((format == BuiltinFormats.TextMap || format == BuiltinFormats.HttpHeaders) &&
                carrier is ITextMap textMap)
            {
                if (spanContext is ApplicationInsightsSpanContext appInsightsContext)
                    _propagator.Inject(appInsightsContext, textMap);
                return;
            }

            throw new ApplicationInsightsFormatException(
                $"Unrecognized carrier format [{format}]. Only ITextMap is supported by this driver.");
        }

        public ISpanContext Extract<TCarrier>(IFormat<TCarrier> format, TCarrier carrier)
        {
            if ((format == BuiltinFormats.TextMap || format == BuiltinFormats.HttpHeaders) &&
                carrier is ITextMap textMap)
                return _propagator.Extract(textMap);

            throw new ApplicationInsightsFormatException(
                $"Unrecognized carrier format [{format}]. Only ITextMap is supported by this driver.");
        }

        public IScopeManager ScopeManager { get; }
        public ISpan ActiveSpan => ScopeManager.Active?.Span ?? NoOpHelpers.NoOpSpan;
    }
}