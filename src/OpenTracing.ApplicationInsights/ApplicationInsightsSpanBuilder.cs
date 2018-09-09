using System;
using OpenTracing.Tag;

namespace OpenTracing.ApplicationInsights
{
    public sealed class ApplicationInsightsSpanBuilder : ISpanBuilder
    {
        public ISpanBuilder AsChildOf(ISpanContext parent)
        {
            throw new NotImplementedException();
        }

        public ISpanBuilder AsChildOf(ISpan parent)
        {
            throw new NotImplementedException();
        }

        public ISpanBuilder AddReference(string referenceType, ISpanContext referencedContext)
        {
            throw new NotImplementedException();
        }

        public ISpanBuilder IgnoreActiveSpan()
        {
            throw new NotImplementedException();
        }

        public ISpanBuilder WithTag(string key, string value)
        {
            throw new NotImplementedException();
        }

        public ISpanBuilder WithTag(string key, bool value)
        {
            throw new NotImplementedException();
        }

        public ISpanBuilder WithTag(string key, int value)
        {
            throw new NotImplementedException();
        }

        public ISpanBuilder WithTag(string key, double value)
        {
            throw new NotImplementedException();
        }

        public ISpanBuilder WithTag(BooleanTag tag, bool value)
        {
            throw new NotImplementedException();
        }

        public ISpanBuilder WithTag(IntOrStringTag tag, string value)
        {
            throw new NotImplementedException();
        }

        public ISpanBuilder WithTag(IntTag tag, int value)
        {
            throw new NotImplementedException();
        }

        public ISpanBuilder WithTag(StringTag tag, string value)
        {
            throw new NotImplementedException();
        }

        public ISpanBuilder WithStartTimestamp(DateTimeOffset timestamp)
        {
            throw new NotImplementedException();
        }

        public IScope StartActive()
        {
            throw new NotImplementedException();
        }

        public IScope StartActive(bool finishSpanOnDispose)
        {
            throw new NotImplementedException();
        }

        public ISpan Start()
        {
            throw new NotImplementedException();
        }
    }
}