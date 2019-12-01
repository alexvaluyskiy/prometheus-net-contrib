using Microsoft.AspNetCore.Http;
using Prometheus.Contrib.Core;
using System.Diagnostics;

namespace Prometheus.Contrib.Diagnostic
{
    public class AspNetCoreListenerHandler : DiagnosticListenerHandler
    {
        private static class PrometheusCounters
        {
            public static readonly Histogram AspNetCoreRequestsDuration = Metrics.CreateHistogram(
                "http_request_duration_seconds",
                "The duration of HTTP requests processed by an ASP.NET Core application.",
                new HistogramConfiguration
                {
                    LabelNames = new[] { "code", "method" },
                    Buckets = Histogram.ExponentialBuckets(0.001, 2, 16)
                });

            public static readonly Counter AspNetCoreRequestsErrors = Metrics.CreateCounter(
                "http_request_errors",
                "Total HTTP requests received errors.");
        }

        public AspNetCoreListenerHandler(string sourceName) : base(sourceName)
        {
        }

        public override void OnStopActivity(Activity activity, object payload)
        {
            if (payload is HttpContext httpContext)
            {
                PrometheusCounters.AspNetCoreRequestsDuration
                   .WithLabels(httpContext.Response.StatusCode.ToString(), httpContext.Request.Method)
                   .Observe(activity.Duration.TotalSeconds);
            }
        }

        public override void OnException(Activity activity, object payload)
        {
            PrometheusCounters.AspNetCoreRequestsErrors.Inc();
        }
    }
}
