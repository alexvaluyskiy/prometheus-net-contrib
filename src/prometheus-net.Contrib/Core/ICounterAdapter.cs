namespace Prometheus.Contrib.Core
{
    public interface ICounterAdapter
    {
        void OnCounterEvent(string name, double value);
    }
}