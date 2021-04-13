using Prometheus.Contrib.Core;
using System.Diagnostics;
using System.Net.Http;

namespace Prometheus.Contrib.Diagnostics
{
    public class HttpClientListenerHandler : DiagnosticListenerHandler
    {
        private readonly IPrometheusCounters counters;
        
        public interface IPrometheusCounters
        {
            ICollector<IHistogram> HttpClientRequestsDuration { get; }
            ICounter HttpClientRequestsErrors { get; }
        }

        public sealed class PrometheusCounters : IPrometheusCounters
        {
            public ICollector<IHistogram> HttpClientRequestsDuration { get; } = Metrics.CreateHistogram(
                "http_client_requests_duration_seconds",
                "Time between first byte of request headers sent to last byte of response received.",
                new HistogramConfiguration
                {
                    LabelNames = new[] { "code", "host" },
                    Buckets = Histogram.ExponentialBuckets(0.001, 2, 16)
                });

            public ICounter HttpClientRequestsErrors { get; } = Metrics.CreateCounter(
                "http_client_requests_errors_total",
                "Total HTTP requests sent errors.");
        }

        private readonly PropertyFetcher<object> stopResponseFetcher = new PropertyFetcher<object>("Response");
        private readonly PropertyFetcher<object> stopRequestFetcher = new PropertyFetcher<object>("Request");

        public HttpClientListenerHandler(string sourceName, IPrometheusCounters counters) : base(sourceName)
        {
            this.counters = counters;
        }

        public override void OnStopActivity(Activity activity, object payload)
        {
            var response = stopResponseFetcher.Fetch(payload);

            if (response is HttpResponseMessage httpResponse)
            {
                counters.HttpClientRequestsDuration
                    .WithLabels(httpResponse.StatusCode.ToString("D"), httpResponse.RequestMessage.RequestUri.Host)
                    .Observe(activity.Duration.TotalSeconds);
                return;
            }

            var request = stopRequestFetcher.Fetch(payload);

            if (request is HttpRequestMessage httpRequest)
            {
                counters.HttpClientRequestsDuration
                    .WithLabels("0", httpRequest.RequestUri.Host)
                    .Observe(activity.Duration.TotalSeconds);
            }
        }

        public override void OnException(Activity activity, object payload)
        {
            counters.HttpClientRequestsErrors.Inc();
        }
    }
}
