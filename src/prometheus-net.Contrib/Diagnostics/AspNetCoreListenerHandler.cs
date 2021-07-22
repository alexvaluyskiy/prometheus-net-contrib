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
                "aspnetcore_requests_duration_seconds",
                "The duration of HTTP requests processed by an ASP.NET Core application.",
                new HistogramConfiguration
                {
                    LabelNames = new[] { "code", "method", "route" },
                    Buckets = Histogram.ExponentialBuckets(0.001, 2, 16)
                });

            public static readonly Counter AspNetCoreRequestsErrors = Metrics.CreateCounter(
                "aspnetcore_requests_errors_total",
                "Total HTTP requests received errors.");

            public static readonly Counter AspNetCoreRequestsTotal = Metrics.CreateCounter(
                "aspnetcore_requests_total",
                "Total Requests.",
                new CounterConfiguration
                {
                    LabelNames = new[] { "code", "method", "route" }
                });

            public static readonly Gauge AspNetCoreRequestsInProgress = Metrics.CreateGauge(
                "aspnetcore_requests_in_progress",
                "Current Requests.",
                labelNames: new[] { "method", "route" });
        }

        private string Route(HttpContext httpContext)
        {
            var endpointFeature = httpContext.Features[typeof(IEndpointFeature)] as IEndpointFeature;
            return endpointFeature?.Endpoint is RouteEndpoint endpoint ? endpoint.RoutePattern.RawText : string.Empty;
        }

        public AspNetCoreListenerHandler(string sourceName) : base(sourceName)
        {
        }

        public override void OnStartActivity(Activity activity, object payload)
        {
            if (payload is HttpContext httpContext)
            {
                PrometheusCounters.AspNetCoreRequestsInProgress
                   .WithLabels(httpContext.Request.Method, Route(httpContext)).Inc();
            }
        }

        public override void OnStopActivity(Activity activity, object payload)
        {
            if (payload is HttpContext httpContext)
            {
                var route = Route(httpContext);

                PrometheusCounters.AspNetCoreRequestsDuration
                   .WithLabels(httpContext.Response.StatusCode.ToString(), httpContext.Request.Method, route)
                   .Observe(activity.Duration.TotalSeconds);

                PrometheusCounters.AspNetCoreRequestsInProgress
                   .WithLabels(httpContext.Request.Method, route).Dec();
            }
        }

        public override void OnException(Activity activity, object payload)
        {
            PrometheusCounters.AspNetCoreRequestsErrors.Inc();

            if (payload is HttpContext httpContext)
            {
                var route = Route(httpContext);

                PrometheusCounters.AspNetCoreRequestsInProgress
                   .WithLabels(httpContext.Request.Method, route).Dec();
            }
        }
    }
}
