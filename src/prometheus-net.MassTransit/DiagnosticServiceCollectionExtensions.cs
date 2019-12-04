using Prometheus.Contrib.Core;
using Prometheus.Contrib.Diagnostic;
using Prometheus.Contrib.Diagnostics;
using Prometheus.Contrib.EventListeners;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DiagnosticServiceCollectionExtensions
    {
        public static void AddPrometheusMassTransitMetrics(this IServiceCollection services)
        {
            var aspNetCoreListenerHandler = new DiagnosticSourceSubscriber(
                name => new MassTransitListenerHandler(name),
                listener => listener.Name.Equals("MassTransit"));
            aspNetCoreListenerHandler.Subscribe();

            services.AddSingleton(aspNetCoreListenerHandler);
        }
    }
}
