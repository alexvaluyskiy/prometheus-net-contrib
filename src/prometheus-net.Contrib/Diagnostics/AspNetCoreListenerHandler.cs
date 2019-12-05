using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Prometheus.Contrib.Core;
using System.Diagnostics;

namespace Prometheus.Contrib.Diagnostics
{
    public class AspNetCoreListenerHandler : DiagnosticListenerHandler
    {
        private static class PrometheusCounters
        {
            public static readonly Histogram AspNetCoreRequestsDuration = Metrics.CreateHistogram(
                "aspnetcore_request_duration_seconds",
                "The duration of HTTP requests processed by an ASP.NET Core application.",
                new HistogramConfiguration
                {
                    LabelNames = new[] { "code", "method", "route" },
                    Buckets = Histogram.ExponentialBuckets(0.001, 2, 16)
                });

            public static readonly Counter AspNetCoreRequestsErrors = Metrics.CreateCounter(
                "aspnetcore_request_errors",
                "Total HTTP requests received errors.");
        }

        public AspNetCoreListenerHandler(string sourceName) : base(sourceName)
        {
        }

        public override void OnStopActivity(Activity activity, object payload)
        {
            if (payload is HttpContext httpContext)
            {
                var endpointFeature = httpContext.Features[typeof(IEndpointFeature)] as IEndpointFeature;
                string route = endpointFeature?.Endpoint is RouteEndpoint endpoint ? endpoint.RoutePattern.RawText : string.Empty;

                PrometheusCounters.AspNetCoreRequestsDuration
                   .WithLabels(httpContext.Response.StatusCode.ToString(), httpContext.Request.Method, route)
                   .Observe(activity.Duration.TotalSeconds);
            }
        }

        public override void OnException(Activity activity, object payload)
        {
            PrometheusCounters.AspNetCoreRequestsErrors.Inc();
        }
    }
}
