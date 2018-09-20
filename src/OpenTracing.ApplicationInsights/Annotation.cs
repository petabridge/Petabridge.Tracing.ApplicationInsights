using System;

namespace OpenTracing.ApplicationInsights
{
    /// <inheritdoc />
    /// <summary>
    ///     A standard string-based annotation.
    /// </summary>
    public struct Annotation
    {
        public Annotation(DateTimeOffset timestamp, string value)
        {
            Timestamp = timestamp;
            Value = value;
        }

        public DateTimeOffset Timestamp { get; }

        public string Value { get; }
    }
}