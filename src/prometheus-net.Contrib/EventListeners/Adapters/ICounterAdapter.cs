namespace Prometheus.Contrib.EventListeners.Adapters
{
    public interface ICounterAdapter
    {
        void OnCounterEvent(string name, double value);
    }
}