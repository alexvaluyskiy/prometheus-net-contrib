using Prometheus.Contrib.Core;
using Prometheus.EntityFramework.Diagnostics;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DiagnosticServiceCollectionExtensions
    {
        public static void AddPrometheusEntityFrameworkMetrics(this IServiceCollection services)
        {
            var entityFrameworkListenerHandler = new DiagnosticSourceSubscriber(
                name => new EntityFrameworkListenerHandler(name),
                listener => listener.Name.Equals("Microsoft.EntityFrameworkCore"));
            entityFrameworkListenerHandler.Subscribe();

            services.AddSingleton(entityFrameworkListenerHandler);
        }
    }
}
