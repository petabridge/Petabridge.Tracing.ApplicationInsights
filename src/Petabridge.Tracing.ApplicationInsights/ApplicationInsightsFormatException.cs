// -----------------------------------------------------------------------
// <copyright file="ApplicationInsightsFormatException.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Petabridge.Tracing.ApplicationInsights
{
    /// <summary>
    ///     Thrown when we have a propagation header or some other data member in an invalid state.
    /// </summary>
    public class ApplicationInsightsFormatException : Exception
    {
        public ApplicationInsightsFormatException(string message) : base(message)
        {
        }

        public ApplicationInsightsFormatException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}