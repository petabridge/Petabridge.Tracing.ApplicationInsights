using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using OpenTracing.Tag;

namespace OpenTracing.ApplicationInsights
{
    /// <inheritdoc />
    /// <summary>
    /// An Application Insights-specific <see cref="T:OpenTracing.ISpan" /> interface.
    /// </summary>
    public interface IApplicationInsightsSpan : ISpan
    {
        IApplicationInsightsSpanContext TypedContext { get; }

        SpanKind SpanKind { get; }
    }

    public abstract class ApplicationInsightsSpanBase : IApplicationInsightsSpan
    {
        public static readonly IReadOnlyDictionary<string, string> EmptyTags = new Dictionary<string, string>();
        protected Dictionary<string, string> _tags;
        protected readonly ApplicationInsightsTracer Tracer;

        protected ApplicationInsightsSpanBase(ApplicationInsightsTracer tracer, IApplicationInsightsSpanContext typedContext, string operationName, DateTimeOffset start, SpanKind spanKind,
            Endpoint localEndpoint = null, Dictionary<string, string> tags = null)
        {
            TypedContext = typedContext;
            SpanKind = spanKind;
            OperationName = operationName;
            Started = start;
            LocalEndpoint = localEndpoint;
            _tags = tags;
            Tracer = tracer;
        }

        public abstract ISpan SetTag(string key, string value);

        public ISpan SetTag(string key, bool value)
        {
            return SetTag(key, Convert.ToString(value));
        }

        public ISpan SetTag(string key, int value)
        {
            return SetTag(key, Convert.ToString(value));
        }

        public ISpan SetTag(string key, double value)
        {
            return SetTag(key, Convert.ToString(value, CultureInfo.InvariantCulture));
        }

        public ISpan SetTag(BooleanTag tag, bool value)
        {
            return SetTag(tag.Key, value);
        }

        public ISpan SetTag(IntOrStringTag tag, string value)
        {
            return SetTag(tag.Key, value);
        }

        public ISpan SetTag(IntTag tag, int value)
        {
            return SetTag(tag.Key, value);
        }

        public ISpan SetTag(StringTag tag, string value)
        {
            return SetTag(tag.Key, value);
        }

        public ISpan Log(IEnumerable<KeyValuePair<string, object>> fields)
        {
            throw new NotImplementedException();
        }

        public ISpan Log(DateTimeOffset timestamp, IEnumerable<KeyValuePair<string, object>> fields)
        {
            throw new NotImplementedException();
        }

        public ISpan Log(string @event)
        {
            throw new NotImplementedException();
        }

        public abstract ISpan Log(DateTimeOffset timestamp, string @event);

        public ISpan SetBaggageItem(string key, string value)
        {
            throw new NotSupportedException("Baggage is not supported in ApplicationInsights");
        }

        public string GetBaggageItem(string key)
        {
            throw new NotSupportedException("Baggage is not supported in ApplicationInsights");
        }

        public ISpan SetOperationName(string operationName)
        {
            OperationName = operationName;
            return this;
        }

        public void Finish()
        {
            Finish(Tracer.TimeProvider.Now);
        }

        public abstract void Finish(DateTimeOffset finishTimestamp);

        public IReadOnlyDictionary<string, string> Tags => _tags ?? EmptyTags;

        /// <summary>
        ///     The local <see cref="Endpoint" />
        /// </summary>
        public Endpoint LocalEndpoint { get; }

        /// <summary>
        ///     The remote <see cref="Endpoint" />. Has to be set by the <see cref="ISpanBuilder" /> or the <see cref="ISpan" />.
        /// </summary>
        public Endpoint RemoteEndpoint { get; private set; }

        /// <summary>
        ///     The name of the operation for this <see cref="ApplicationInsightsSpanBase" />
        /// </summary>
        public string OperationName { get; private set; }

        /// <summary>
        ///     The start time of this operation.
        /// </summary>
        public DateTimeOffset Started { get; }

        /// <summary>
        ///     The completion time of this operation.
        /// </summary>
        public DateTimeOffset? Finished { get; private set; }

        /// <summary>
        ///     The duration of this operation.
        /// </summary>
        public TimeSpan? Duration
        {
            get
            {
                if (!Finished.HasValue) return null;
                return Finished - Started;
            }
        }

        public ISpanContext Context => TypedContext;
        public IApplicationInsightsSpanContext TypedContext { get; }
        public SpanKind SpanKind { get; }
    }
}