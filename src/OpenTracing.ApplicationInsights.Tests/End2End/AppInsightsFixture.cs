// -----------------------------------------------------------------------
// <copyright file="AppInsightsFixture.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility;

namespace OpenTracing.ApplicationInsights.Tests.End2End
{
    public class AppInsightsFixture : IDisposable
    {
        private const string URL =
            "https://api.applicationinsights.io/v1/apps/{0}/query?query={1}";

        public AppInsightsFixture()
        {
            /*
             * Needed for querying the Azure App Insights REST API: 
             *
             */
            ApiKey = Environment.GetEnvironmentVariable("APP_INSIGHTS_KEY");
            AppId = Environment.GetEnvironmentVariable("APP_INSIGHTS_APPID");

            if (string.IsNullOrEmpty(ApiKey) || string.IsNullOrEmpty(AppId))
                throw new ArgumentNullException(
                    $"Couldn't find values for environment variable [APP_INSIGHTS_APPID] or [APP_INSIGHTS_KEY]");

            var instrumentationKey = Environment.GetEnvironmentVariable("APP_INSIGHTS_INSTRUMENTATION_KEY");

            if (string.IsNullOrEmpty(instrumentationKey))
                throw new ArgumentNullException(
                    $"Couldn't find value for environment variable [APP_INSIGHTS_INSTRUMENTATION_KEY].");

            TelemetryConfiguration = new TelemetryConfiguration(instrumentationKey);

            AppInsightsClient = new HttpClient();
            AppInsightsClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            AppInsightsClient.DefaultRequestHeaders.Add("x-api-key", ApiKey);
        }

        public string ApiKey { get; }

        public string AppId { get; }

        public TelemetryConfiguration TelemetryConfiguration { get; }

        /// <summary>
        ///     HTTP client with pre-configured settings designed to target https://dev.applicationinsights.io
        /// </summary>
        public HttpClient AppInsightsClient { get; }

        public void Dispose()
        {
            TelemetryConfiguration.Dispose();
            AppInsightsClient.Dispose();
        }

        public async Task<(string response, bool isSuccess)> QueryOperationsForTraceId(string traceId)
        {
            var request = string.Format(URL, AppId,
                Uri.EscapeUriString($"union * | where operation_Id == \"{traceId}\""));

            var response = await AppInsightsClient.GetAsync(request);

            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadAsStringAsync(), true);
            return (response.ReasonPhrase, false);
        }
    }
}