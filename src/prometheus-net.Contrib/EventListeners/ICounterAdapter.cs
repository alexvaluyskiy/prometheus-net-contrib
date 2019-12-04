namespace Prometheus.Contrib.EventListeners
{
    public interface ICounterAdapter
    {
        void OnCounterEvent(string name, double value);
    }
}