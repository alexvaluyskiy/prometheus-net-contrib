using Prometheus.Contrib.Core;
using System.Diagnostics;
using System.Net.Http;

namespace Prometheus.Contrib.Diagnostics
{
    public class HttpClientListenerHandler : DiagnosticListenerHandler
    {
        private static class PrometheusCounters
        {
            public static readonly Histogram HttpClientRequestsDuration = Metrics.CreateHistogram(
                "http_client_requests_duration_seconds",
                "Time between first byte of request headers sent to last byte of response received.",
                new HistogramConfiguration
                {
                    LabelNames = new[] { "code", "host" },
                    Buckets = Histogram.ExponentialBuckets(0.001, 2, 16)
                });

            public static readonly Counter HttpClientRequestsErrors = Metrics.CreateCounter(
                "http_client_requests_errors_total",
                "Total HTTP requests sent errors.");
        }

        private readonly PropertyFetcher<HttpResponseMessage> stopResponseFetcher = new PropertyFetcher<HttpResponseMessage>("Response");
        
        private readonly PropertyFetcher<HttpRequestMessage> stopRequestFetcher = new PropertyFetcher<HttpRequestMessage>("Request");

        public HttpClientListenerHandler(string sourceName) : base(sourceName)
        {
        }

        public override void OnStopActivity(Activity activity, object payload)
        { 
            if(stopResponseFetcher.TryFetch(payload, out HttpResponseMessage httpResponse) && httpResponse != null)
                PrometheusCounters.HttpClientRequestsDuration
                    .WithLabels(httpResponse.StatusCode.ToString("D"), httpResponse.RequestMessage.RequestUri.Host)
                    .Observe(activity.Duration.TotalSeconds);
            
            else if(stopRequestFetcher.TryFetch(payload, out HttpRequestMessage httpRequest) && httpRequest != null)
                PrometheusCounters.HttpClientRequestsDuration
                    .WithLabels("0", httpRequest.RequestUri.Host)
                    .Observe(activity.Duration.TotalSeconds);
        }

        public override void OnException(Activity activity, object payload)
        {
            PrometheusCounters.HttpClientRequestsErrors.Inc();
        }
    }
}
