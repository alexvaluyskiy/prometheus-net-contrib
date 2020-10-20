using Prometheus.Contrib.Core;
using System.Diagnostics;
using System.Net.Http;

namespace Prometheus.Contrib.Diagnostics
{
    public class GrpcClientListenerHandler : DiagnosticListenerHandler
    {
        private static class PrometheusCounters
        {
            public static readonly Histogram GrpcClientRequestsDuration = Metrics.CreateHistogram(
                "grpc_client_requests_duration_seconds",
                "Time between first byte of request headers sent to last byte of response received.",
                new HistogramConfiguration
                {
                    LabelNames = new[] { "code", "host" },
                    Buckets = Histogram.ExponentialBuckets(0.001, 2, 16)
                });

            public static readonly Counter GrpcClientRequestsErrors = Metrics.CreateCounter(
                "grpc_client_requests_errors_total",
                "Total GRPC requests sent errors.");
        }

        private readonly PropertyFetcher<object> stopResponseFetcher = new PropertyFetcher<object>("Response");

        public GrpcClientListenerHandler(string sourceName) : base(sourceName)
        {
        }

        public override void OnStopActivity(Activity activity, object payload)
        {
            var response = stopResponseFetcher.Fetch(payload);

            if (response is HttpResponseMessage httpResponse)
                PrometheusCounters.GrpcClientRequestsDuration
                    .WithLabels(httpResponse.StatusCode.ToString("D"), httpResponse.RequestMessage.RequestUri.Host)
                    .Observe(activity.Duration.TotalSeconds);
        }

        public override void OnException(Activity activity, object payload)
        {
            PrometheusCounters.GrpcClientRequestsErrors.Inc();
        }
    }
}
