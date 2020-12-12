using Prometheus.Contrib.EventListeners.Counters;

namespace Prometheus.Contrib.EventListeners.Adapters
{
    internal class PrometheusNetSocketsCounterAdapter : BaseAdapter
    {
        public const string EventSourceName = "System.Net.Sockets";

        internal readonly MeanCounter OutgoingConnectionsEstablished = new MeanCounter("outgoing-connections-established", "net_sockets_outgoing_connections_total", "Outgoing Connections Established");
        internal readonly MeanCounter IncomingConnectionsEstablished = new MeanCounter("incoming-connections-established", "net_sockets_incoming_connections_total", "Incoming Connections Established");
        internal readonly MeanCounter BytesReceived = new MeanCounter("bytes-received", "net_sockets_bytes_received", "Bytes Received");
        internal readonly MeanCounter BytesSent = new MeanCounter("bytes-sent", "net_sockets_bytes_sent", "Bytes Sent");
        internal readonly MeanCounter DatagramsReceived = new MeanCounter("datagrams-received", "net_sockets_datagrams_received", "Datagrams Received");
        internal readonly MeanCounter DatagramsSent = new MeanCounter("datagrams-sent", "net_sockets_datagrams_sent", "Datagrams Sent");
    }
}