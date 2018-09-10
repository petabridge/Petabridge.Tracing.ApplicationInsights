using System;

namespace OpenTracing.ApplicationInsights.Util
{
    /// <inheritdoc />
    /// <summary>
    ///     Creates new timestamps via simply new-ing up a <see cref="T:System.DateTimeOffset" />.
    /// </summary>
    public sealed class DateTimeOffsetTimeProvider : ITimeProvider
    {
        public DateTimeOffset Now => DateTimeOffset.UtcNow;
    }
}