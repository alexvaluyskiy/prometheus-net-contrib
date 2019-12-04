using Prometheus.Contrib.Core;
using Prometheus.Contrib.Diagnostic;
using Prometheus.Contrib.EventListeners;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DiagnosticServiceCollectionExtensions
    {
        public static void AddPrometheusMonitoring(this IServiceCollection services)
        {
            var httpClientListenerHandler = new DiagnosticSourceSubscriber(
                name => new HttpClientListenerHandler(name),
                listener => listener.Name.Equals("HttpHandlerDiagnosticListener"));
            httpClientListenerHandler.Subscribe();

            var aspNetCoreListenerHandler = new DiagnosticSourceSubscriber(
                name => new AspNetCoreListenerHandler(name),
                listener => listener.Name.Equals("Microsoft.AspNetCore"));
            aspNetCoreListenerHandler.Subscribe();

            var massTransitListenerHandler = new DiagnosticSourceSubscriber(
                name => new MassTransitListenerHandler(name),
                listener => listener.Name.Equals("MassTransit"));
            massTransitListenerHandler.Subscribe();

            var sqlClientListenerHandler = new DiagnosticSourceSubscriber(
                name => new SqlClientListenerHandler(name),
                listener => listener.Name.Equals("SqlClientDiagnosticListener"));
            sqlClientListenerHandler.Subscribe();

            var entityFrameworkListenerHandler = new DiagnosticSourceSubscriber(
                name => new EntityFrameworkListenerHandler(name),
                listener => listener.Name.Equals("Microsoft.EntityFrameworkCore"));
            entityFrameworkListenerHandler.Subscribe();
        }

        public static void AddPrometheusCounters(this IServiceCollection services)
        {
            services.AddSingleton(new CountersEventListener());
        }
    }
}
