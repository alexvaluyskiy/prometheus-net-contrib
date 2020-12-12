using Prometheus.Contrib.EventListeners.Counters;

namespace Prometheus.Contrib.EventListeners.Adapters
{
    internal class PrometheusGrpcClientCounterAdapter : BaseAdapter
    {
        public const string EventSourceName = "Grpc.Net.Client";

        internal readonly MeanCounter TotalCalls = new MeanCounter("total-calls", "grpc_client_calls_total", "Total Calls");
        internal readonly MeanCounter CurrentCalls = new MeanCounter("current-calls", "grpc_client_calls_current_total", "Current Calls");
        internal readonly MeanCounter CallsFailed = new MeanCounter("calls-failed", "grpc_client_calls_failed_total", "Total Calls Failed");
        internal readonly MeanCounter CallsDeadlineExceeded = new MeanCounter("calls-deadline-exceeded", "grpc_client_calls_deadline_exceeded_total", "Total Calls Deadline Exceeded");
        internal readonly MeanCounter MessagesSent = new MeanCounter("messages-sent", "grpc_client_messages_sent_total", "Total Messages Sent");
        internal readonly MeanCounter MessagesReceived = new MeanCounter("messages-received", "grpc_client_messages_received_total", "Total Messages Received");
    }
}
