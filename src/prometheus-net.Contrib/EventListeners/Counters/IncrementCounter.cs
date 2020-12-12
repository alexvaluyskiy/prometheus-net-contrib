using System.Collections.Generic;

namespace Prometheus.Contrib.EventListeners.Counters
{
    internal class IncrementCounter : BaseCounter
    {
        private static Gauge _metric;

        public IncrementCounter(string name, string displayName, string description) : base(name, displayName, description)
        {
            _metric = Metrics.CreateGauge(DisplayName, Description);
        }
        
        public override bool TryReadEventCounterData(IDictionary<string, object> eventData)
        {
            if (!eventData.TryGetValue("Increment", out var increment))
                return false;
            
            _metric.Set((double)increment);
            return true;
        }
    }
}