using Prometheus;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;

namespace Prometheus.Contrib.EventListeners
{
    public class SignalREventListener : EventListener
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

        private IDictionary<string, string> eventArguments;
        private const string EventSourceName = "Microsoft.AspNetCore.Http.Connections";

        public SignalREventListener(TimeSpan interval)
        {
            eventArguments = new Dictionary<string, string>
            {
                { "EventCounterIntervalSec", interval.TotalSeconds.ToString() }
            };
        }

        protected override void OnEventSourceCreated(EventSource source)
        {
            if (source.Name.Equals(EventSourceName))
            {
                EnableEvents(source, EventLevel.Verbose, EventKeywords.All, eventArguments);
            }
        }

        private (string Name, double Value) GetRelevantMetric(IDictionary<string, object> eventPayload)
        {
            string counterName = "";
            double counterValue = 0;

            foreach (KeyValuePair<string, object> payload in eventPayload)
            {
                string key = payload.Key;
                string val = payload.Value.ToString();

                if (key.Equals("Name"))
                {
                    counterName = val;
                }
                else if (key.Equals("Mean") || key.Equals("Increment"))
                {
                    counterValue = double.Parse(val);
                }
            }

            return (counterName, counterValue);
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            if (!(eventData.EventName.Equals("EventCounters") && eventData.EventSource.Name.Equals(EventSourceName)))
            {
                return;
            }

            foreach (var payload in eventData.Payload)
            {
                if (payload is IDictionary<string, object> eventPayload)
                {
                    var counterKV = GetRelevantMetric(eventPayload);
                    switch (counterKV.Name)
                    {
                        case SignalrCountersConstants.SignalrConnectionsStarted:
                            SignalrPrometheusCounters.SignalrConnectionsStarted.Set(counterKV.Value);
                            break;
                        case SignalrCountersConstants.SignalrConnectionsStopped:
                            SignalrPrometheusCounters.SignalrConnectionsStopped.Set(counterKV.Value);
                            break;
                        case SignalrCountersConstants.SignalrConnectionsTimedOut:
                            SignalrPrometheusCounters.SignalrConnectionsTimedOut.Set(counterKV.Value);
                            break;
                        case SignalrCountersConstants.SignalrCurrentConnections:
                            SignalrPrometheusCounters.SignalrCurrentConnections.Set(counterKV.Value);
                            break;
                        case SignalrCountersConstants.SignalrConnectionsDuration:
                            SignalrPrometheusCounters.SignalrConnectionsDuration.Set(counterKV.Value);
                            break;
                    }
                }
            }
        }
    }
}
