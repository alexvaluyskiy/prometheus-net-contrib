using Prometheus.Contrib.Core;
using Prometheus.EasyCaching.Diagnostics;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DiagnosticServiceCollectionExtensions
    {
        public static void AddPrometheusEasyCachingMetrics(this IServiceCollection services)
        {
            var easyCachingListenerHandler = new DiagnosticSourceSubscriber(
                name => new EasyCachingListenerHandler(name),
                listener => listener.Name.Equals("EasyCachingDiagnosticListener"));
            easyCachingListenerHandler.Subscribe();

            services.AddSingleton(easyCachingListenerHandler);
        }
    }
}
