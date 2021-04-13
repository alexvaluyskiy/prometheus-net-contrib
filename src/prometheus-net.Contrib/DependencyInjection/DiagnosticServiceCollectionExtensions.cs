using System;
using Prometheus;
using Prometheus.Contrib.Core;
using Prometheus.Contrib.Diagnostics;
using Prometheus.Contrib.EventListeners;
using Prometheus.Contrib.Options;

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

        public static void AddPrometheusHttpClientMetrics(this IServiceCollection services, HttpClientListenerHandler.IPrometheusCounters counters)
        {
            counters ??= new HttpClientListenerHandler.PrometheusCounters();
            
            var httpClientListenerHandler = new DiagnosticSourceSubscriber(
                name => new HttpClientListenerHandler(name, counters),
                listener => listener.Name.Equals("HttpHandlerDiagnosticListener"));
            httpClientListenerHandler.Subscribe();

            services.AddSingleton(httpClientListenerHandler);
        }

        public static void AddPrometheusSqlClientMetrics(this IServiceCollection services, Action<SqlMetricsOptions> optionsInvoker = null)
        {
            var sqlMetricsOptions = new SqlMetricsOptions();
            optionsInvoker?.Invoke(sqlMetricsOptions);

            var sqlClientListenerHandler = new DiagnosticSourceSubscriber(
                name => new SqlClientListenerHandler(name, sqlMetricsOptions),
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

        public static void AddPrometheusCounters(this IServiceCollection services, int refreshPeriodSeconds = 10)
        {
            services.AddSingleton(new CountersEventListener(refreshPeriodSeconds));
        }
    }
}
