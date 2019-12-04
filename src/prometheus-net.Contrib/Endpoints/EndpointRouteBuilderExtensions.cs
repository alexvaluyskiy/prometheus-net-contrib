using Microsoft.AspNetCore.Builder;
using Prometheus;
using Prometheus.Contrib.Endpoints;

namespace Microsoft.AspNetCore.Routing
{
    public static class EndpointRouteBuilderExtensions
    {
        public static IEndpointConventionBuilder MapMetrics(this IEndpointRouteBuilder endpoints, string pattern = "metrics", CollectorRegistry registry = null)
        {
            var pipeline = endpoints.CreateApplicationBuilder()
                .UseMiddleware<MetricEndpointMiddleware>(new MetricEndpointMiddleware.Settings
                {
                    Registry = registry
                })
                .Build();

            return endpoints.Map(pattern, pipeline).WithDisplayName("Prometheus Metrics");
        }
    }
}
