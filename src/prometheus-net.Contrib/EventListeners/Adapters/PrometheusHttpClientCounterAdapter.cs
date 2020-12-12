using Prometheus.Contrib.EventListeners.Counters;

namespace Prometheus.Contrib.EventListeners.Adapters
{
    internal class PrometheusHttpClientCounterAdapter : BaseAdapter
    {
        public const string EventSourceName = "System.Net.Http";
            
        internal readonly MeanCounter RequestsStarted = new MeanCounter("requests-started", "http_client_requests_started_total", "Requests Started");
        internal readonly IncrementCounter RequestsStartedRate = new IncrementCounter("requests-started-rate", "http_client_requests_started_per_second", "Requests Started Rate");
        internal readonly MeanCounter RequestsAborted = new MeanCounter("requests-aborted", "http_client_requests_aborted_total", "Total Requests Aborted");
        internal readonly IncrementCounter RequestsAbortedRate = new IncrementCounter("requests-aborted-rate", "http_client_requests_aborted_per_second", "Requests Failed Rate");
        internal readonly MeanCounter CurrentRequests = new MeanCounter("current-requests", "http_client_requests_current_total", "Current Requests");
        internal readonly MeanCounter Http11ConnectionsCurrentTotal = new MeanCounter("http11-connections-current-total", "http_client_http11_connections_current_total", "Current Http 1.1 Connections");
        internal readonly MeanCounter Http20ConnectionsCurrentTotal = new MeanCounter("http20-connections-current-total", "http_client_http20_connections_current_total", "Current Http 2.0 Connections");
        internal readonly MeanCounter Http11RequestsQueueDuration = new MeanCounter("http11-requests-queue-duration", "http_client_http11_requests_queue_duration", "HTTP 1.1 Requests Queue Duration");
        internal readonly MeanCounter Http20RequestsQueueDuration = new MeanCounter("http20-requests-queue-duration", "http_client_http20_requests_queue_duration", "HTTP 2.0 Requests Queue Duration");
    }
}
