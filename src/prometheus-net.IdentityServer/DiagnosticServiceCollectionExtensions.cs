using IdentityServer4.Services;
using Prometheus.IdentityServer.Sinks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DiagnosticServiceCollectionExtensions
    {
        public static void AddPrometheusIdentityServerMetrics(this IServiceCollection services)
        {
            services.AddSingleton<IEventSink, PrometheusEventsSink>();
        }
    }
}
