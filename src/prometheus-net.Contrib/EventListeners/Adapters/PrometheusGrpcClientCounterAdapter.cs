using Prometheus.Contrib.Core;

namespace Prometheus.Contrib.EventListeners.Adapters
{
    public class PrometheusGrpcClientCounterAdapter : ICounterAdapter
    {
        private static class GrpcClientCountersConstants
        {
            public const string GrpcClientTotalCalls = "total-calls";
            public const string GrpcClientCurrentCalls = "current-calls";
            public const string GrpcClientCallsFailed = "calls-failed";
            public const string GrpcClientCallsDeadlineExceeded = "calls-deadline-exceeded";
            public const string GrpcClientMessagesSent = "messages-sent";
            public const string GrpcClientMessagesReceived = "messages-received";
        }

        private static class GrpcClientPrometheusCounters
        {
            public static Gauge GrpcClientTotalCalls = Metrics.CreateGauge("grpc_client_counters_total_calls", "Total Calls");
            public static Gauge GrpcClientCurrentCalls = Metrics.CreateGauge("grpc_client_counters_current_calls", "Current Calls");
            public static Gauge GrpcClientCallsFailed = Metrics.CreateGauge("grpc_client_counters_calls_failed", "Total Calls Failed");
            public static Gauge GrpcClientCallsDeadlineExceeded = Metrics.CreateGauge("grpc_client_counters_calls_deadline_exceeded", "Total Calls Deadline Exceeded");
            public static Gauge GrpcClientMessagesSent = Metrics.CreateGauge("grpc_client_counters_messages_sent", "Total Messages Sent");
            public static Gauge GrpcClientMessagesReceived = Metrics.CreateGauge("grpc_client_counters_messages_received", "Total Messages Received");
        }

        public void OnCounterEvent(string name, double value)
        {
            switch (name)
            {
                case GrpcClientCountersConstants.GrpcClientTotalCalls:
                    GrpcClientPrometheusCounters.GrpcClientTotalCalls.Set(value);
                    break;
                case GrpcClientCountersConstants.GrpcClientCurrentCalls:
                    GrpcClientPrometheusCounters.GrpcClientCurrentCalls.Set(value);
                    break;
                case GrpcClientCountersConstants.GrpcClientCallsFailed:
                    GrpcClientPrometheusCounters.GrpcClientCallsFailed.Set(value);
                    break;
                case GrpcClientCountersConstants.GrpcClientCallsDeadlineExceeded:
                    GrpcClientPrometheusCounters.GrpcClientCallsDeadlineExceeded.Set(value);
                    break;
                case GrpcClientCountersConstants.GrpcClientMessagesSent:
                    GrpcClientPrometheusCounters.GrpcClientMessagesSent.Set(value);
                    break;
                case GrpcClientCountersConstants.GrpcClientMessagesReceived:
                    GrpcClientPrometheusCounters.GrpcClientMessagesReceived.Set(value);
                    break;
            }
        }
    }
}
