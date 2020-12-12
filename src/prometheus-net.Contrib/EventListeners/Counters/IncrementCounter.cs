using System.Collections.Generic;

namespace Prometheus.Contrib.EventListeners.Counters
{
    internal class IncrementCounter : BaseCounter
    {
        public IncrementCounter(string name, string displayName, string description) : base(name, displayName, description)
        {
            Metric = Metrics.CreateGauge(DisplayName, Description);
        }
        
        internal Gauge Metric { get; }

        public override bool TryReadEventCounterData(IDictionary<string, object> eventData)
        {
            if (!eventData.TryGetValue("Increment", out var increment))
                return false;
            
            Metric.Set((double)increment);
            return true;
        }
    }
}