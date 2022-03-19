// -----------------------------------------------------------------------
// <copyright file="RequestSpan.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using OpenTracing;

namespace Petabridge.Tracing.ApplicationInsights
{
    /// <inheritdoc />
    /// <summary>
    ///     Used to process <see cref="T:Microsoft.ApplicationInsights.DataContracts.DependencyTelemetry" /> events.
    /// </summary>
    public sealed class RequestSpan : ApplicationInsightsSpan
    {
        private IOperationHolder<RequestTelemetry> _operation;

        public RequestSpan(ApplicationInsightsTracer tracer, IApplicationInsightsSpanContext typedContext,
            string operationName, DateTimeOffset start, SpanKind spanKind, Endpoint localEndpoint = null,
            Dictionary<string, string> tagsActual = null) : base(tracer, typedContext, operationName, start, spanKind,
            localEndpoint, tagsActual)
        {
            var telemetry = new RequestTelemetry {Id = typedContext.SpanId, Name = operationName};
            InitializeTelemetry(typedContext, localEndpoint, tagsActual, telemetry);

            _operation = Tracer.Client.StartOperation(telemetry);
        }

        public override IDictionary<string, string> Tags => _operation.Telemetry.Properties ?? EmptyTags;

        public override ISpan SetTag(string key, string value)
        {
            // guard the trace so we don't collect garbage
            if (!Finished.HasValue)
            {
                _operation.Telemetry.Properties[key] = value;
                
                // need to flag this as an error in Application Insights
                if (key.Equals(OpenTracing.Tag.Tags.Error.Key))
                {
                    _operation.Telemetry.Success = false;
                }
            }

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