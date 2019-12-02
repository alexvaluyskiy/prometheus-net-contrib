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
            //httpClientListenerHandler.Subscribe();

            var aspNetCoreListenerHandler = new DiagnosticSourceSubscriber(
                name => new AspNetCoreListenerHandler(name),
                listener => listener.Name.Equals("Microsoft.AspNetCore"));
            //aspNetCoreListenerHandler.Subscribe();

            var massTransitListenerHandler = new DiagnosticSourceSubscriber(
                name => new MassTransitListenerHandler(name),
                listener => listener.Name.Equals("MassTransit"));
            //massTransitListenerHandler.Subscribe();

            var sqlClientListenerHandler = new DiagnosticSourceSubscriber(
                name => new SqlClientListenerHandler(name),
                listener => listener.Name.Equals("SqlClientDiagnosticListener"));
            sqlClientListenerHandler.Subscribe();

            var entityFrameworkListenerHandler = new DiagnosticSourceSubscriber(
                name => new SqlClientListenerHandler(name),
                listener => listener.Name.Equals("SqlClientDiagnosticListener"));
            entityFrameworkListenerHandler.Subscribe();

            var listener = new RuntimeEventListener(TimeSpan.FromSeconds(10));
            var aspNetCoreListener = new AspNetCoreEventListener(TimeSpan.FromSeconds(10));
            var signalrListener = new SignalREventListener(TimeSpan.FromSeconds(10));
        }
    }
}
