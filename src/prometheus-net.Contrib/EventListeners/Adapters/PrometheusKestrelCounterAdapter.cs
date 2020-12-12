using System.Collections.Generic;
using Prometheus.Contrib.Core;
using Prometheus.Contrib.EventListeners.Counters;

namespace Prometheus.Contrib.EventListeners.Adapters
{
    internal class PrometheusKestrelCounterAdapter : ICounterAdapter
    {
        public const string EventSourceName = "Microsoft-AspNetCore-Server-Kestrel";
            
        internal readonly IncrementCounter ConnectionPerSecond = new IncrementCounter("connections-per-second", "kestrel_connections_per_second", "Number of connections between update intervals");
        internal readonly MeanCounter TotalConnections = new MeanCounter("total-connections", "kestrel_connections_total", "Total Requests");
        internal readonly MeanCounter CurrentConnections = new MeanCounter("current-connections", "kestrel_connections_current_total", "Number of current connections");
        internal readonly IncrementCounter TlsHandshakesPerSecond = new IncrementCounter("tls-handshakes-per-second", "kestrel_tls_handshakes_per_second", "Number of TLS Handshakes made between update intervals");
        internal readonly MeanCounter TotalTlsHandshakes = new MeanCounter("total-tls-handshakes", "kestrel_tls_handshakes_total", "Total number of TLS handshakes made");
        internal readonly MeanCounter CurrentTlsHandshakes = new MeanCounter("current-tls-handshakes", "kestrel_tls_handshakes_current_total", "Number of currently active TLS handshakes");
        internal readonly MeanCounter FailedTlsHandshakes = new MeanCounter("failed-tls-handshakes","kestrel_tls_handshakes_failed_total", "Total number of failed TLS handshakes");
        internal readonly MeanCounter ConnectionQueueLength = new MeanCounter("connection-queue-length", "kestrel_connections_queue_total", "Length of Kestrel Connection Queue");
        internal readonly MeanCounter RequestQueueLength = new MeanCounter("request-queue-length", "kestrel_requests_queue_total", "Length total HTTP request queue");
        internal readonly MeanCounter CurrentUpgradedRequests = new MeanCounter("current-upgraded-requests", "kestrel_requests_upgraded_total", "Current Upgraded Requests (WebSockets)");

        private readonly Dictionary<string, BaseCounter> _counters;

        public PrometheusKestrelCounterAdapter()
        {
            _counters = CounterUtils.GenerateDictionary(this);
        }

        public void OnCounterEvent(IDictionary<string, object> eventPayload)
        {
            if (!eventPayload.TryGetValue("Name", out var counterName))
            {
                return;
            }
            
            if (!_counters.TryGetValue((string) counterName, out var counter))
                return;

            counter.TryReadEventCounterData(eventPayload);
        }
    }
}
