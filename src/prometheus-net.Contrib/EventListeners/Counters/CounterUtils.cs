using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Prometheus.Contrib.EventListeners.Counters
{
    public class CounterUtils
    {
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