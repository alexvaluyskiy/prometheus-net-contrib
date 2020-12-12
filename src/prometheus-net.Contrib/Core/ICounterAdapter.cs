using System.Collections.Generic;

namespace Prometheus.Contrib.Core
{
    public interface ICounterAdapter
    {
        void OnCounterEvent(IDictionary<string, object> eventPayload);
    }
}