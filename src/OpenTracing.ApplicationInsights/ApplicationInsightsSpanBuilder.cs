using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using OpenTracing.ApplicationInsights.Util;
using OpenTracing.Tag;

namespace OpenTracing.ApplicationInsights
{
    /// <summary>
    /// Builder interface for creating <see cref="IApplicationInsightsSpan"/> instances.
    /// </summary>
    public sealed class ApplicationInsightsSpanBuilder : ISpanBuilder
    {
        private readonly ApplicationInsightsTracer _tracer;
        private readonly string _operationName;
        private Dictionary<string, string> _initialTags;
        private List<SpanReference> _references;
        private SpanKind _spanKind = SpanKind.SERVER;
        private DateTimeOffset? _start;
        private bool _ignoreActive;

        public ApplicationInsightsSpanBuilder(ApplicationInsightsTracer tracer, string operationName)
        {
            _tracer = tracer;
            _operationName = operationName;
        }

        public ISpanBuilder AsChildOf(ISpanContext parent)
        {
            return AddReference(References.ChildOf, parent);
        }

        public ISpanBuilder AsChildOf(ISpan parent)
        {
            return AsChildOf(parent.Context);
        }

        public ISpanBuilder AddReference(string referenceType, ISpanContext referencedContext)
        {
            if (!referencedContext.IsAppInsightsSpan()) return this; // stop execution here
            if (_references == null) _references = new List<SpanReference>();
            _references.Add(new SpanReference(referenceType, referencedContext));
            return this;
        }

        public ISpanBuilder IgnoreActiveSpan()
        {
            _ignoreActive = true;
            return this;
        }

        public ISpanBuilder WithTag(string key, string value)
        {
            if (_initialTags == null) _initialTags = new Dictionary<string, string>();
            _initialTags[key] = value;
            return this;
        }

        public ISpanBuilder WithTag(string key, bool value)
        {
            return WithTag(key, Convert.ToString(value));
        }

        public ISpanBuilder WithTag(string key, int value)
        {
            return WithTag(key, Convert.ToString(value));
        }

        public ISpanBuilder WithTag(string key, double value)
        {
            return WithTag(key, Convert.ToString(value, CultureInfo.InvariantCulture));
        }

        public ISpanBuilder WithTag(BooleanTag tag, bool value)
        {
            return WithTag(tag.Key, value);
        }

        public ISpanBuilder WithTag(IntOrStringTag tag, string value)
        {
            return WithTag(tag.Key, value);
        }

        public ISpanBuilder WithTag(IntTag tag, int value)
        {
            return WithTag(tag.Key, value);
        }

        public ISpanBuilder WithTag(StringTag tag, string value)
        {
            return WithTag(tag.Key, value);
        }

        public ISpanBuilder WithStartTimestamp(DateTimeOffset timestamp)
        {
            _start = timestamp;
            return this;
        }

        public IScope StartActive()
        {
            return _tracer.ScopeManager.Activate(Start(), true);
        }

        public IScope StartActive(bool finishSpanOnDispose)
        {
            return _tracer.ScopeManager.Activate(Start(), finishSpanOnDispose);
        }

        public ISpan Start()
        {
            if (_start == null)
                _start = _tracer.TimeProvider.Now;

            var activeSpanContext = _tracer.ActiveSpan?.Context;
            ApplicationInsightsSpanContext parentContext = null;

            if (_references != null && (_ignoreActive || activeSpanContext.IsEmpty()))
                parentContext = FindBestFittingReference(_references);
            else if (!activeSpanContext.IsEmpty())
                parentContext = (ApplicationInsightsSpanContext)activeSpanContext;

        }

        private static ApplicationInsightsSpanContext FindBestFittingReference(IReadOnlyList<SpanReference> references)
        {
            foreach (var reference in references)
                if (reference.ReferenceType.Equals(References.ChildOf))
                    return (ApplicationInsightsSpanContext)reference.SpanContext;

            return (ApplicationInsightsSpanContext)references.First().SpanContext;
        }

        /// <summary>
        ///     INTERNAL API. Used to describe a relationship between <see cref="ApplicationInsightsSpan" /> instances.
        /// </summary>
        private struct SpanReference : IEquatable<SpanReference>
        {
            public SpanReference(string referenceType, ISpanContext spanContext)
            {
                ReferenceType = referenceType;
                SpanContext = spanContext;
            }

            public string ReferenceType { get; }

            public ISpanContext SpanContext { get; }

            public bool Equals(SpanReference other)
            {
                return string.Equals(ReferenceType, other.ReferenceType)
                       && SpanContext.Equals(other.SpanContext);
            }

            public override bool Equals(object obj)
            {
                if (obj is null) return false;
                return obj is SpanReference reference && Equals(reference);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (ReferenceType.GetHashCode() * 397) ^ SpanContext.GetHashCode();
                }
            }

            public static bool operator ==(SpanReference left, SpanReference right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(SpanReference left, SpanReference right)
            {
                return !left.Equals(right);
            }
        }
    }
}