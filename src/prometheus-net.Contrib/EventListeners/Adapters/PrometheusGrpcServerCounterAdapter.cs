using Prometheus.Contrib.EventListeners.Counters;

namespace Prometheus.Contrib.EventListeners.Adapters
{
    internal class PrometheusGrpcServerCounterAdapter : BaseAdapter
    {
        public const string EventSourceName = "Grpc.AspNetCore.Server";

        internal readonly MeanCounter TotalCalls = new MeanCounter("total-calls", "grpc_server_calls_total", "Total Calls");
        internal readonly MeanCounter CurrentCalls = new MeanCounter("current-calls", "grpc_server_calls_current_total", "Current Calls");
        internal readonly MeanCounter CallsFailed = new MeanCounter("calls-failed", "grpc_server_calls_failed_total", "Total Calls Failed");
        internal readonly MeanCounter CallsDeadlineExceeded = new MeanCounter("calls-deadline-exceeded", "grpc_server_deadline_exceeded_total", "Total Calls Deadline Exceeded");
        internal readonly MeanCounter MessagesSent = new MeanCounter("messages-sent", "grpc_server_messages_sent_total", "Total Messages Sent");
        internal readonly MeanCounter MessagesReceived = new MeanCounter("messages-received", "grpc_server_messages_received_total", "Total Messages Received");
        internal readonly MeanCounter CallsUnimplemented = new MeanCounter("calls-unimplemented", "grpc_server_calls_unimplemented_total", "Total Calls Unimplemented");
    }
}
