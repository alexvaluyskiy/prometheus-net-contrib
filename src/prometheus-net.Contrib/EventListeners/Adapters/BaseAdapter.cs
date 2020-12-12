using System.Collections.Generic;
using Prometheus.Contrib.Core;
using Prometheus.Contrib.EventListeners.Counters;

namespace Prometheus.Contrib.EventListeners.Adapters
{
    public abstract class BaseAdapter : ICounterAdapter
    {
        private readonly Dictionary<string, BaseCounter> _counters;

        protected BaseAdapter()
        {
            _counters = CounterUtils.GenerateDictionary(this);
        }

        public void OnCounterEvent(IDictionary<string, object> eventPayload)
        {
            if (!eventPayload.TryGetValue("Name", out var counterName))
            {
                return;
            }
            
            if (!_counters.TryGetValue((string) counterName, out var counter))
                return;

            counter.TryReadEventCounterData(eventPayload);
        }
    }
}