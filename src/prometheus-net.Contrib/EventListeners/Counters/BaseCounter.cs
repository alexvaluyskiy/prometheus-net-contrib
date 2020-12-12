using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
        
        internal static Dictionary<string, BaseCounter> GenerateDictionary<TFrom>(TFrom owningType)
        {
            return owningType.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic)
                .Where(x => x.FieldType.BaseType == typeof(BaseCounter))
                .Select(x => x.GetValue(owningType) as BaseCounter)
                .ToDictionary(k => k.Name, k => k);
        }
    }
}