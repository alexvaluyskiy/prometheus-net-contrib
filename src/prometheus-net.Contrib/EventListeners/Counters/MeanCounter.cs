using System.Collections.Generic;

namespace Prometheus.Contrib.EventListeners.Counters
{
    internal class MeanCounter : BaseCounter
    {
        private readonly Gauge _metric;

        public MeanCounter(string name, string displayName, string description) : base(name, displayName, description)
        {
            _metric = Metrics.CreateGauge(DisplayName, Description);
        }
        
        public override bool TryReadEventCounterData(IDictionary<string, object> eventData)
        {
            if (!(eventData.TryGetValue("Mean", out var meanObj) && meanObj is double mean)
                || !(eventData.TryGetValue("Count", out var countObj) && countObj is int count))
            {
                return false;
            }

            var val = mean == 0 && count > 0 ? count : mean;
            val = double.IsNaN(val) ? 0 : val;
            
            _metric.Set(val);
            
            return true;
        }
    }
}