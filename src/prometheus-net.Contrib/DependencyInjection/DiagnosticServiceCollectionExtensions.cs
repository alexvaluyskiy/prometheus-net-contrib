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

        public static void AddPrometheusSqlClientMetrics(this IServiceCollection services)
        {
            var sqlClientListenerHandler = new DiagnosticSourceSubscriber(
                name => new SqlClientListenerHandler(name),
                listener => listener.Name.Equals("SqlClientDiagnosticListener"));
            sqlClientListenerHandler.Subscribe();

            services.AddSingleton(sqlClientListenerHandler);
        }

        public static void AddPrometheusGrpcClientMetrics(this IServiceCollection services)
        {
            var grpcClientListenerHandler = new DiagnosticSourceSubscriber(
                name => new GrpcClientListenerHandler(name),
                listener => listener.Name.Equals("Grpc.Net.Client"));
            grpcClientListenerHandler.Subscribe();

            services.AddSingleton(grpcClientListenerHandler);
        }

        public static void AddPrometheusCounters(this IServiceCollection services)
        {
            services.AddSingleton(new CountersEventListener());
        }
    }
}
