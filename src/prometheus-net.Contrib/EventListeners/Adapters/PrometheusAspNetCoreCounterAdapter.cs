using Prometheus.Contrib.EventListeners.Counters;

namespace Prometheus.Contrib.EventListeners.Adapters
{
    internal class PrometheusAspNetCoreCounterAdapter : BaseAdapter
    {
        public const string EventSourceName = "Microsoft.AspNetCore.Hosting";
            
        internal readonly IncrementCounter RequestsPerSecond = new IncrementCounter("requests-per-second", "aspnetcore_requests_per_second", "Request Rate");
        internal readonly MeanCounter TotalRequests = new MeanCounter("total-requests","aspnetcore_requests_total", "Total Requests");
        internal readonly MeanCounter CurrentRequests = new MeanCounter("current-requests", "aspnetcore_requests_current_total", "Current Requests");
        internal readonly MeanCounter FailedRequests = new MeanCounter("failed-requests", "aspnetcore_requests_failed_total", "Failed Requests");
    }
}
