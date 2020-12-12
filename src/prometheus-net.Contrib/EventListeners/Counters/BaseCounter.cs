using System.Collections.Generic;

namespace Prometheus.Contrib.EventListeners.Counters
{
    internal abstract class BaseCounter
    {
        protected BaseCounter(string name, string displayName, string description)
        {
            Name = name;
            DisplayName = displayName;
            Description = description;
        }
        
        public string Name { get; }
        protected string DisplayName { get; }
        protected string Description { get; }

        public abstract bool TryReadEventCounterData(IDictionary<string, object> eventData);
    }
}