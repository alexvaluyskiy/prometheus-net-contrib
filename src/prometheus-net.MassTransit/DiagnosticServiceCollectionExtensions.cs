using MassTransit;
using Prometheus.Contrib.Core;
using Prometheus.MassTransit.Diagnostics;
using Prometheus.MassTransit.Observers;

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

        public static void ConnectPrometheusObservers(this IBusControl busControl)
        {
            busControl.ConnectPublishObserver(new PrometheusPublishObserver());
            busControl.ConnectReceiveObserver(new PrometheusConsumeObserver());
            busControl.ConnectSendObserver(new PrometheusSendObserver());
        }
    }
}
