using Prometheus.Contrib.Core;

namespace Prometheus.Contrib.EventListeners.Adapters
{
    public class PrometheusGrpcServerCounterAdapter : ICounterAdapter
    {
        private static class GrpcServerCountersConstants
        {
            public const string GrpcServerTotalCalls = "total-calls";
            public const string GrpcServerCurrentCalls = "current-calls";
            public const string GrpcServerCallsFailed = "calls-failed";
            public const string GrpcServerCallsDeadlineExceeded = "calls-deadline-exceeded";
            public const string GrpcServerMessagesSent = "messages-sent";
            public const string GrpcServerMessagesReceived = "messages-received";
            public const string GrpcServerCallsUnimplemented = "calls-unimplemented";
        }

        private static class GrpcServerPrometheusCounters
        {
            public static Gauge GrpcServerTotalCalls = Metrics.CreateGauge("grpc_server_counters_total_calls", "Total Calls");
            public static Gauge GrpcServerCurrentCalls = Metrics.CreateGauge("grpc_server_counters_current_calls", "Current Calls");
            public static Gauge GrpcServerCallsFailed = Metrics.CreateGauge("grpc_server_counters_calls_failed", "Total Calls Failed");
            public static Gauge GrpcServerCallsDeadlineExceeded = Metrics.CreateGauge("grpc_server_counters_calls_deadline_exceeded", "Total Calls Deadline Exceeded");
            public static Gauge GrpcServerMessagesSent = Metrics.CreateGauge("grpc_server_counters_messages_sent", "Total Messages Sent");
            public static Gauge GrpcServerMessagesReceived = Metrics.CreateGauge("grpc_server_counters_messages_received", "Total Messages Received");
            public static Gauge GrpcServerCallsUnimplemented = Metrics.CreateGauge("grpc_server_counters_calls_unimplemented", "Total Calls Unimplemented");
        }

        public void OnCounterEvent(string name, double value)
        {
            switch (name)
            {
                case GrpcServerCountersConstants.GrpcServerTotalCalls:
                    GrpcServerPrometheusCounters.GrpcServerTotalCalls.Set(value);
                    break;
                case GrpcServerCountersConstants.GrpcServerCurrentCalls:
                    GrpcServerPrometheusCounters.GrpcServerCurrentCalls.Set(value);
                    break;
                case GrpcServerCountersConstants.GrpcServerCallsFailed:
                    GrpcServerPrometheusCounters.GrpcServerCallsFailed.Set(value);
                    break;
                case GrpcServerCountersConstants.GrpcServerCallsDeadlineExceeded:
                    GrpcServerPrometheusCounters.GrpcServerCallsDeadlineExceeded.Set(value);
                    break;
                case GrpcServerCountersConstants.GrpcServerMessagesSent:
                    GrpcServerPrometheusCounters.GrpcServerMessagesSent.Set(value);
                    break;
                case GrpcServerCountersConstants.GrpcServerMessagesReceived:
                    GrpcServerPrometheusCounters.GrpcServerMessagesReceived.Set(value);
                    break;
                case GrpcServerCountersConstants.GrpcServerCallsUnimplemented:
                    GrpcServerPrometheusCounters.GrpcServerCallsUnimplemented.Set(value);
                    break;
            }
        }
    }
}
