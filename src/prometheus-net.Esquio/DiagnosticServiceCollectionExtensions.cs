using Prometheus.Contrib.Core;
using Prometheus.Esquio.Diagnostics;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DiagnosticServiceCollectionExtensions
    {
        public static void AddPrometheusMassTransitMetrics(this IServiceCollection services)
        {
            var esquioListenerHandler = new DiagnosticSourceSubscriber(
                name => new EsquioListenerHandler(name),
                listener => listener.Name.Equals("Esquio"));
            esquioListenerHandler.Subscribe();

            services.AddSingleton(esquioListenerHandler);
        }
    }
}
