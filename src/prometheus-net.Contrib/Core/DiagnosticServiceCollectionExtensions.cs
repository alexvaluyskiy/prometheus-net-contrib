using Prometheus.Contrib.Core;
using Prometheus.Contrib.Diagnostics;
using Prometheus.Contrib.EventListeners;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DiagnosticServiceCollectionExtensions
    {
        public static void AddPrometheusAspNetCoreMetrics(this IServiceCollection services)
        {
            var aspNetCoreListenerHandler = new DiagnosticSourceSubscriber(
                name => new AspNetCoreListenerHandler(name),
                listener => listener.Name.Equals("Microsoft.AspNetCore"));
            aspNetCoreListenerHandler.Subscribe();

            services.AddSingleton(aspNetCoreListenerHandler);
        }

        public static void AddPrometheusHttpClientMetrics(this IServiceCollection services)
        {
            var httpClientListenerHandler = new DiagnosticSourceSubscriber(
                name => new HttpClientListenerHandler(name),
                listener => listener.Name.Equals("HttpHandlerDiagnosticListener"));
            httpClientListenerHandler.Subscribe();

            services.AddSingleton(httpClientListenerHandler);
        }

        public static void AddPrometheusEntityFrameworkMetrics(this IServiceCollection services)
        {
            var entityFrameworkListenerHandler = new DiagnosticSourceSubscriber(
                name => new EntityFrameworkListenerHandler(name),
                listener => listener.Name.Equals("Microsoft.EntityFrameworkCore"));
            entityFrameworkListenerHandler.Subscribe();

            services.AddSingleton(entityFrameworkListenerHandler);
        }

        public static void AddPrometheusSqlClientMetrics(this IServiceCollection services)
        {
            var sqlClientListenerHandler = new DiagnosticSourceSubscriber(
                name => new SqlClientListenerHandler(name),
                listener => listener.Name.Equals("SqlClientDiagnosticListener"));
            sqlClientListenerHandler.Subscribe();

            services.AddSingleton(sqlClientListenerHandler);
        }

        public static void AddPrometheusCounters(this IServiceCollection services)
        {
            services.AddSingleton(new CountersEventListener());
        }
    }
}
