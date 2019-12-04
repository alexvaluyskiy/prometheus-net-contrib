namespace Prometheus.Contrib.EventListeners.Adapters
{
    public class PrometheusSignalRCounterAdapter : ICounterAdapter
    {
        private static class SignalrCountersConstants
        {
            public const string SignalrConnectionsStarted = "connections-started";
            public const string SignalrConnectionsStopped = "connections-stopped";
            public const string SignalrConnectionsTimedOut = "connections-timed-out";
            public const string SignalrCurrentConnections = "current-connections";
            public const string SignalrConnectionsDuration = "connections-duration";
        }

        private static class SignalrPrometheusCounters
        {
            public static Gauge SignalrConnectionsStarted = Metrics.CreateGauge("signalr_counters_connections_started", "Total Connections Started");
            public static Gauge SignalrConnectionsStopped = Metrics.CreateGauge("signalr_counters_connections_stopped", "Total Connections Stopped");
            public static Gauge SignalrConnectionsTimedOut = Metrics.CreateGauge("signalr_counters_connections_timed_out", "Total Connections Timed Out");
            public static Gauge SignalrCurrentConnections = Metrics.CreateGauge("signalr_counters_current_connections", "Current Connections");
            public static Gauge SignalrConnectionsDuration = Metrics.CreateGauge("signalr_counters_connections_duration", "Average Connection Duration");
        }

        public void OnCounterEvent(string name, double value)
        {
            switch (name)
            {
                case SignalrCountersConstants.SignalrConnectionsStarted:
                    SignalrPrometheusCounters.SignalrConnectionsStarted.Set(value);
                    break;
                case SignalrCountersConstants.SignalrConnectionsStopped:
                    SignalrPrometheusCounters.SignalrConnectionsStopped.Set(value);
                    break;
                case SignalrCountersConstants.SignalrConnectionsTimedOut:
                    SignalrPrometheusCounters.SignalrConnectionsTimedOut.Set(value);
                    break;
                case SignalrCountersConstants.SignalrCurrentConnections:
                    SignalrPrometheusCounters.SignalrCurrentConnections.Set(value);
                    break;
                case SignalrCountersConstants.SignalrConnectionsDuration:
                    SignalrPrometheusCounters.SignalrConnectionsDuration.Set(value);
                    break;
            }
        }
    }
}
