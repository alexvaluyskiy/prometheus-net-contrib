using Prometheus.Contrib.Core;
using System.Diagnostics;
using System.Net.Http;

namespace Prometheus.Contrib.Diagnostic
{
    public class HttpClientListenerHandler : DiagnosticListenerHandler
    {
        private static class PrometheusCounters
        {
            public static readonly Histogram HttpClientRequestsDuration = Metrics.CreateHistogram(
                "httpclient_requests_duration_seconds",
                "Time between first byte of request headers sent to last byte of response received.",
                new HistogramConfiguration
                {
                    LabelNames = new[] { "code", "host" },
                    Buckets = Histogram.ExponentialBuckets(0.001, 2, 16)
                });

            public static readonly Counter HttpClientRequestsErrors = Metrics.CreateCounter(
                "httpclient_requests_errors",
                "Total HTTP requests sent errors.");
        }

        private readonly PropertyFetcher stopResponseFetcher = new PropertyFetcher("Response");

        public HttpClientListenerHandler(string sourceName) : base(sourceName)
        {
        }

        public override void OnStopActivity(Activity activity, object payload)
        {
            var response = stopResponseFetcher.Fetch(payload);

            if (response is HttpResponseMessage httpResponse)
            {
                PrometheusCounters.HttpClientRequestsDuration
                    .WithLabels(httpResponse.StatusCode.ToString("D"), httpResponse.RequestMessage.RequestUri.Host)
                    .Observe(activity.Duration.TotalSeconds);
            }
        }

        public override void OnException(Activity activity, object payload)
        {
            PrometheusCounters.HttpClientRequestsErrors.Inc();
        }
    }
}
