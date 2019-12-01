using Prometheus;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;

namespace Prometheus.Contrib.EventListeners
{
    public class AspNetCoreEventListener : EventListener
    {
        private static class AspNetCoreCountersConstants
        {
            public const string AspNetCoreRequestsPerSecond = "requests-per-second";
            public const string AspNetCoreTotalRequests = "total-requests";
            public const string AspNetCoreCurrentRequests = "current-requests";
            public const string AspNetCoreFailedRequests = "failed-requests";
        }

        private static class AspNetCorePrometheusCounters
        {
            public static Gauge AspNetCoreRequestsPerSecond = Metrics.CreateGauge("aspnetcore_counters_requests_per_second", "Request Rate");
            public static Gauge AspNetCoreTotalRequests = Metrics.CreateGauge("aspnetcore_counters_total_requests", "Total Requests");
            public static Gauge AspNetCoreCurrentRequests = Metrics.CreateGauge("aspnetcore_counters_current_requests", "Current Requests");
            public static Gauge AspNetCoreFailedRequests = Metrics.CreateGauge("aspnetcore_counters_failed_requests", "Failed Requests");
        }

        private IDictionary<string, string> eventArguments;
        private const string EventSourceName = "Microsoft.AspNetCore.Hosting";

        public AspNetCoreEventListener(TimeSpan interval)
        {
            eventArguments = new Dictionary<string, string>
            {
                { "EventCounterIntervalSec", interval.TotalSeconds.ToString() }
            };
        }

        protected override void OnEventSourceCreated(EventSource source)
        {
            if (source.Name.Equals(EventSourceName))
                EnableEvents(source, EventLevel.Verbose, EventKeywords.All, eventArguments);
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
                    counterName = val;
                else if (key.Equals("Mean") || key.Equals("Increment"))
                    counterValue = double.Parse(val);
            }

            return (counterName, counterValue);
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            if (!(eventData.EventName.Equals("EventCounters") && eventData.EventSource.Name.Equals(EventSourceName)))
                return;

            foreach (var payload in eventData.Payload)
                if (payload is IDictionary<string, object> eventPayload)
                {
                    var counterKV = GetRelevantMetric(eventPayload);
                    switch (counterKV.Name)
                    {
                        case AspNetCoreCountersConstants.AspNetCoreRequestsPerSecond:
                            AspNetCorePrometheusCounters.AspNetCoreRequestsPerSecond.Set(counterKV.Value);
                            break;
                        case AspNetCoreCountersConstants.AspNetCoreTotalRequests:
                            AspNetCorePrometheusCounters.AspNetCoreTotalRequests.Set(counterKV.Value);
                            break;
                        case AspNetCoreCountersConstants.AspNetCoreCurrentRequests:
                            AspNetCorePrometheusCounters.AspNetCoreCurrentRequests.Set(counterKV.Value);
                            break;
                        case AspNetCoreCountersConstants.AspNetCoreFailedRequests:
                            AspNetCorePrometheusCounters.AspNetCoreFailedRequests.Set(counterKV.Value);
                            break;
                    }
                }
        }
    }
}
