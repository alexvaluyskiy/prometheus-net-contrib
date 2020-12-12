using Prometheus.Contrib.EventListeners.Counters;

namespace Prometheus.Contrib.EventListeners.Adapters
{
    internal class PrometheusNetNameResolutionCounterAdapter : BaseAdapter
    {
        public const string EventSourceName = "System.Net.NameResolution";

        internal readonly MeanCounter DnsLookupsRequested = new MeanCounter("dns-lookups-requested", "net_dns_lookups_requested_total", "DNS Lookups Requested");
        internal readonly MeanCounter DnsLookupsDuration = new MeanCounter("dns-lookups-duration", "net_dns_lookups_duration_milliseconds", "Average DNS Lookup Duration");
    }
}