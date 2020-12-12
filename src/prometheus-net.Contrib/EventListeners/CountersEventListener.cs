using System;
using Prometheus.Contrib.EventListeners.Adapters;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Prometheus.Contrib.Core;

namespace Prometheus.Contrib.EventListeners
{
    internal class CountersEventListener : EventListener
    {
        private readonly int refreshPeriodSeconds;

        private readonly Dictionary<string, ICounterAdapter> counterAdapters = new Dictionary<string, ICounterAdapter>
        {
            [PrometheusRuntimeCounterAdapter.EventSourceName] = new PrometheusRuntimeCounterAdapter(),
            [PrometheusAspNetCoreCounterAdapter.EventSourceName] = new PrometheusAspNetCoreCounterAdapter(),
            [PrometheusSignalRCounterAdapter.EventSourceName] = new PrometheusSignalRCounterAdapter(),
            [PrometheusGrpcServerCounterAdapter.EventSourceName] = new PrometheusGrpcServerCounterAdapter(),
            [PrometheusGrpcClientCounterAdapter.EventSourceName] = new PrometheusGrpcClientCounterAdapter(),
            [PrometheusEfCoreCounterAdapter.EventSourceName] = new PrometheusEfCoreCounterAdapter(),
            [PrometheusKestrelCounterAdapter.EventSourceName] = new PrometheusKestrelCounterAdapter(),
            [PrometheusHttpClientCounterAdapter.EventSourceName] = new PrometheusHttpClientCounterAdapter(),
            [PrometheusNetSecurityCounterAdapter.EventSourceName] = new PrometheusNetSecurityCounterAdapter(),
            [PrometheusNetNameResolutionCounterAdapter.EventSourceName] = new PrometheusNetNameResolutionCounterAdapter()
        };

        internal CountersEventListener(int refreshPeriodSeconds = 10)
        {
            this.refreshPeriodSeconds = refreshPeriodSeconds;
            
            EventSourceCreated += OnEventSourceCreated;
        }

        private void OnEventSourceCreated(object sender, EventSourceCreatedEventArgs e)
        {
            if (e.EventSource == null || !counterAdapters.ContainsKey(e.EventSource.Name))
            {
                return;
            }
            
            var args = new Dictionary<string, string>
            {
                ["EventCounterIntervalSec"] = refreshPeriodSeconds.ToString()
            };

            EnableEvents(e.EventSource, EventLevel.Verbose, EventKeywords.All, args);
        }
        
        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            if (eventData.EventName is null || !eventData.EventName.Equals("EventCounters") || eventData.Payload == null)
            {
                return;
            }
            
            if (!counterAdapters.ContainsKey(eventData.EventSource.Name))
            {
                return;
            }

            foreach (var payload in eventData.Payload)
            {
                if (payload is IDictionary<string, object> eventPayload && counterAdapters.TryGetValue(eventData.EventSource.Name, out var adapter))
                {
                    adapter.OnCounterEvent(eventPayload);
                }
            }
        }
    }
}
