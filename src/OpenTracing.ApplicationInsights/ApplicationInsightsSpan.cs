// -----------------------------------------------------------------------
// <copyright file="ApplicationInsightsSpan.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.ApplicationInsights;
using OpenTracing.Tag;

namespace OpenTracing.ApplicationInsights
{
    /// <inheritdoc />
    /// <summary>
    ///     An Application Insights-specific <see cref="T:OpenTracing.ISpan" /> interface.
    /// </summary>
    public interface IApplicationInsightsSpan : ISpan
    {
        IApplicationInsightsSpanContext TypedContext { get; }

        SpanKind SpanKind { get; }

        TimeSpan? Duration { get; }

        string OperationName { get; }

        IReadOnlyDictionary<string, string> Tags { get; }

        /// <summary>
        ///     The local <see cref="Endpoint" />
        /// </summary>
        Endpoint LocalEndpoint { get; }

        /// <summary>
        ///     The remote <see cref="Endpoint" />. Has to be set by the <see cref="ISpanBuilder" /> or the <see cref="ISpan" />.
        /// </summary>
        Endpoint RemoteEndpoint { get; }

        IApplicationInsightsSpan SetRemoteEndpoint(Endpoint remote);
    }

    /// <summary>
    ///     Concrete <see cref="ISpan" /> implementation for Application Insights.
    ///     Contains all metadata needed to be properly recorded by the <see cref="TelemetryClient" />.
    /// </summary>
    public sealed class ApplicationInsightsSpan : IApplicationInsightsSpan
    {
        public static readonly IReadOnlyDictionary<string, string> EmptyTags = new Dictionary<string, string>();
        private readonly ApplicationInsightsTracer _tracer;
        private List<Annotation> _annotations;
        private Dictionary<string, string> _tagsActual;

        public ApplicationInsightsSpan(ApplicationInsightsTracer tracer, IApplicationInsightsSpanContext typedContext,
            string operationName, DateTimeOffset start, SpanKind spanKind,
            Endpoint localEndpoint = null, Dictionary<string, string> tagsActual = null)
        {
            TypedContext = typedContext;
            SpanKind = spanKind;
            OperationName = operationName;
            Started = start;
            LocalEndpoint = localEndpoint;
            _tagsActual = tagsActual;
            _tracer = tracer;
        }

        /// <summary>
        ///     The start time of this operation.
        /// </summary>
        public DateTimeOffset Started { get; }

        /// <summary>
        ///     The completion time of this operation.
        /// </summary>
        public DateTimeOffset? Finished { get; private set; }

        public ISpan SetTag(string key, string value)
        {
            _tagsActual = _tagsActual ?? new Dictionary<string, string>();
            _tagsActual[key] = value;
            return this;
        }

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
            return Log(_tracer.TimeProvider.Now, fields);
        }

        public ISpan Log(DateTimeOffset timestamp, IEnumerable<KeyValuePair<string, object>> fields)
        {
            return Log(timestamp, MergeFields(fields));
        }

        public ISpan Log(string @event)
        {
            return Log(_tracer.TimeProvider.Now, @event);
        }

        public ISpan Log(DateTimeOffset timestamp, string @event)
        {
            return Annotate(timestamp, @event);
        }

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
            Finish(_tracer.TimeProvider.Now);
        }

        public void Finish(DateTimeOffset finishTimestamp)
        {
            if (!Finished.HasValue)
            {
                Finished = finishTimestamp;
                _tracer.Report(this);
            }
        }

        public IReadOnlyDictionary<string, string> Tags => _tagsActual ?? EmptyTags;

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

        /// <summary>
        ///     Set the remote endpoint involved in this operation.
        /// </summary>
        /// <param name="remote">The remote endpoint. Can be a client or a server address.</param>
        /// <returns>This span.</returns>
        public IApplicationInsightsSpan SetRemoteEndpoint(Endpoint remote)
        {
            RemoteEndpoint = remote;
            return this;
        }

        internal static string MergeFields(IEnumerable<KeyValuePair<string, object>> fields)
        {
            return string.Join(" ", fields.Select(entry => entry.Key + ":" + entry.Value));
        }

        internal ISpan Annotate(DateTimeOffset time, string annotationValue)
        {
            _annotations = _annotations ?? new List<Annotation>();
            _annotations.Add(new Annotation(time, annotationValue));
            return this;
        }
    }
}