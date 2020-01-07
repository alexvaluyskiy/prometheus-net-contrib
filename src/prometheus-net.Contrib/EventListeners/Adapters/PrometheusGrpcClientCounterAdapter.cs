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
            public static Gauge GrpcClientTotalCalls = Metrics.CreateGauge("grpc_client_calls_total", "Total Calls");
            public static Gauge GrpcClientCurrentCalls = Metrics.CreateGauge("grpc_client_calls_current_total", "Current Calls");
            public static Gauge GrpcClientCallsFailed = Metrics.CreateGauge("grpc_client_calls_failed_total", "Total Calls Failed");
            public static Gauge GrpcClientCallsDeadlineExceeded = Metrics.CreateGauge("grpc_client_calls_deadline_exceeded_total", "Total Calls Deadline Exceeded");
            public static Gauge GrpcClientMessagesSent = Metrics.CreateGauge("grpc_client_messages_sent_total", "Total Messages Sent");
            public static Gauge GrpcClientMessagesReceived = Metrics.CreateGauge("grpc_client_messages_received_total", "Total Messages Received");
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
