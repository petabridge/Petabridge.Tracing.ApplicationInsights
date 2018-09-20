using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.ApplicationInsights.Extensibility;
using Xunit;

namespace OpenTracing.ApplicationInsights.Tests.End2End
{
    class AppInsightsIntegrationSpec
    {
    }

    public class AppInsightsFixture : IDisposable
    {
        public string ApiKey { get; }

        public string AppId { get; }

        public TelemetryConfiguration TelemetryConfiguration { get; }

        /// <summary>
        /// HTTP client with pre-configured settings designed to target https://dev.applicationinsights.io
        /// </summary>
        public HttpClient AppInsightsClient { get; }

        public AppInsightsFixture()
        {
            ApiKey = Environment.GetEnvironmentVariable("APP_INSIGHTS_KEY");
            AppId = Environment.GetEnvironmentVariable("APP_INSIGHTS_APPID");

            if(string.IsNullOrEmpty(ApiKey) || string.IsNullOrEmpty(AppId))
                throw new ArgumentNullException($"Couldn't find values for environment variable [APP_INSIGHTS_APPID] or [APP_INSIGHTS_KEY]");

            TelemetryConfiguration = new TelemetryConfiguration(AppId);

            AppInsightsClient = new HttpClient();
            AppInsightsClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            AppInsightsClient.DefaultRequestHeaders.Add("x-api-key", ApiKey);
        }

        public void Dispose()
        {
            TelemetryConfiguration.Dispose();
            AppInsightsClient.Dispose();
        }
    }
}
