using System;
using System.Collections.Generic;
using System.Text;

namespace OpenTracing.ApplicationInsights
{
    /// <summary>
    ///     Abstraction for providing time, so we can use virtual time and other fun stuff
    ///     when unit testing.
    /// </summary>
    public interface ITimeProvider
    {
        DateTimeOffset Now { get; }
    }
}
