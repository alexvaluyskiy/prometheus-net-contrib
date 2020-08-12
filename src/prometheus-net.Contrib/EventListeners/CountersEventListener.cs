using Prometheus.Contrib.EventListeners.Adapters;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Prometheus.Contrib.Core;

namespace Prometheus.Contrib.EventListeners
{
    public class CountersEventListener : EventListener
    {
        private readonly Dictionary<string, ICounterAdapter> counterAdapters = new Dictionary<string, ICounterAdapter>
        {
            ["System.Runtime"] = new PrometheusRuntimeCounterAdapter(),
            ["Microsoft.AspNetCore.Hosting"] = new PrometheusAspNetCoreCounterAdapter(),
            ["Microsoft.AspNetCore.Http.Connections"] = new PrometheusSignalRCounterAdapter(),
            ["Grpc.AspNetCore.Server"] = new PrometheusGrpcServerCounterAdapter(),
            ["Grpc.Net.Client"] = new PrometheusGrpcServerCounterAdapter(),
            ["Microsoft.EntityFrameworkCore"] = new PrometheusEfCoreCounterAdapter()
        };

        private readonly IDictionary<string, string> eventArguments = new Dictionary<string, string>
        {
            ["EventCounterIntervalSec"] = "10"
        };

        protected override void OnEventSourceCreated(EventSource source)
        {
            if (counterAdapters.ContainsKey(source.Name))
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
                    counterName = val;
                else if (key.Equals("Mean") || key.Equals("Increment"))
                    counterValue = double.Parse(val);
            }

            return (counterName, counterValue);
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            if (!(eventData.EventName.Equals("EventCounters") && counterAdapters.ContainsKey(eventData.EventSource.Name)))
            {
                return;
            }

            foreach (var payload in eventData.Payload)
            {
                if (payload is IDictionary<string, object> eventPayload && counterAdapters.TryGetValue(eventData.EventSource.Name, out var adapter))
                {
                    var counterKV = GetRelevantMetric(eventPayload);
                    adapter.OnCounterEvent(counterKV.Name, counterKV.Value);
                }
            }
        }
    }
}
