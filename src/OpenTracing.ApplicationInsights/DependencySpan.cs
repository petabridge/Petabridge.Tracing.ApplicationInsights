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
        private readonly IOperationHolder<DependencyTelemetry> _operation;

        public DependencySpan(ApplicationInsightsTracer tracer, IApplicationInsightsSpanContext typedContext,
            string operationName, DateTimeOffset start, SpanKind spanKind, Endpoint localEndpoint = null,
            Dictionary<string, string> tagsActual = null)
            : base(tracer, typedContext, operationName, start, spanKind, localEndpoint, tagsActual)
        {
            _operation = Tracer.Client.StartOperation<DependencyTelemetry>(operationName);
            _operation.Telemetry.Id = typedContext.SpanId;

            InitializeTelemetry(typedContext, localEndpoint, tagsActual, _operation.Telemetry);
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
            if (!Finished.HasValue) Tracer.Client.Track(new TraceTelemetry {Message = @event, Timestamp = timestamp});

            return this;
        }

        protected override void FinishInternal()
        {
            Tracer.Client.StopOperation(_operation);
        }
    }
}