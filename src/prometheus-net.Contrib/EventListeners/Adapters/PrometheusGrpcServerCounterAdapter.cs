using System.Collections.Generic;
using Prometheus.Contrib.Core;

namespace Prometheus.Contrib.EventListeners.Adapters
{
    internal class PrometheusGrpcServerCounterAdapter : ICounterAdapter
    {
        public const string EventSourceName = "Grpc.AspNetCore.Server";

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
            public static Gauge GrpcServerTotalCalls = Metrics.CreateGauge("grpc_server_calls_total", "Total Calls");
            public static Gauge GrpcServerCurrentCalls = Metrics.CreateGauge("grpc_server_calls_current_total", "Current Calls");
            public static Gauge GrpcServerCallsFailed = Metrics.CreateGauge("grpc_server_calls_failed_total", "Total Calls Failed");
            public static Gauge GrpcServerCallsDeadlineExceeded = Metrics.CreateGauge("grpc_server_deadline_exceeded_total", "Total Calls Deadline Exceeded");
            public static Gauge GrpcServerMessagesSent = Metrics.CreateGauge("grpc_server_messages_sent_total", "Total Messages Sent");
            public static Gauge GrpcServerMessagesReceived = Metrics.CreateGauge("grpc_server_messages_received_total", "Total Messages Received");
            public static Gauge GrpcServerCallsUnimplemented = Metrics.CreateGauge("grpc_server_calls_unimplemented_total", "Total Calls Unimplemented");
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

        public void OnCounterEvent(IDictionary<string, object> eventPayload)
        {
        }
    }
}
