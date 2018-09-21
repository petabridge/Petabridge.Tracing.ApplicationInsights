// -----------------------------------------------------------------------
// <copyright file="DependencySpan.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace OpenTracing.ApplicationInsights
{
    /// <inheritdoc />
    /// <summary>
    ///     Used to process <see cref="T:Microsoft.ApplicationInsights.DataContracts.DependencyTelemetry" /> events.
    /// </summary>
    public sealed class DependencySpan : ApplicationInsightsSpan
    {
        private IOperationHolder<DependencyTelemetry> _operation;

        public DependencySpan(ApplicationInsightsTracer tracer, IApplicationInsightsSpanContext typedContext,
            string operationName, DateTimeOffset start, SpanKind spanKind, Endpoint localEndpoint = null,
            Dictionary<string, string> tagsActual = null)
            : base(tracer, typedContext, operationName, start, spanKind, localEndpoint, tagsActual)
        {
            var telemetry = new DependencyTelemetry {Id = typedContext.SpanId, Name = operationName};

            InitializeTelemetry(typedContext, localEndpoint, tagsActual, telemetry);

            _operation = Tracer.Client.StartOperation(telemetry);
        }

        public override IDictionary<string, string> Tags => _operation.Telemetry.Properties ?? EmptyTags;

        public override ISpan SetTag(string key, string value)
        {
            // guard the trace so we don't collect garbage
            if (!Finished.HasValue) _operation.Telemetry.Properties[key] = value;

            return this;
        }

        public override ISpan Log(DateTimeOffset timestamp, string @event)
        {
            // guard the trace so we don't collect garbage
            if (!Finished.HasValue) Tracer.Client.TrackTrace(LogEvent(timestamp, @event));

            return this;
        }

        protected override void FinishInternal()
        {
            if (Duration.HasValue) // should always be true by this point
                _operation.Telemetry.Duration = Duration.Value;

            Tracer.Client.StopOperation(_operation);
            _operation.Dispose();
            _operation = null;
        }
    }
}