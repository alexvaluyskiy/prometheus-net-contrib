using System;
using System.Diagnostics;
using System.Threading;
using Prometheus.Contrib.Core;

namespace Prometheus.EasyCaching.Diagnostics
{
    public class EasyCachingListenerHandler : DiagnosticListenerHandler
    {
        private static class PrometheusCounters
        {
            public static readonly Histogram EasyCachingOperationDuration = Metrics.CreateHistogram(
                "easycaching_operation_duration_seconds",
                "The duration of EasyCaching operation.",
                new HistogramConfiguration()
                {
                    LabelNames = new[] { "operation" }
                });
        }

        private readonly PropertyFetcher timestampFetcher = new PropertyFetcher("Timestamp");

        private readonly AsyncLocal<long> writeSetLocal = new AsyncLocal<long>();
        private readonly AsyncLocal<long> writeGetLocal = new AsyncLocal<long>();

        public EasyCachingListenerHandler(string sourceName) : base(sourceName)
        {
        }
        public override void OnCustom(string name, Activity activity, object payload)
        {
            switch (activity.OperationName)
            {
                case "EasyCaching.WriteSetCacheBefore" when timestampFetcher.Fetch(payload) is long timestamp:
                    this.writeSetLocal.Value = timestamp;
                    break;
                case "EasyCaching.WriteSetCacheAfter" when timestampFetcher.Fetch(payload) is long timestamp:
                    {
                        var beforeTimeStamp = this.writeSetLocal.Value;

                        if (beforeTimeStamp > 0)
                        {
                            var elapsed = TimeSpan.FromTicks(timestamp - beforeTimeStamp).TotalSeconds;

                            PrometheusCounters.EasyCachingOperationDuration
                                .WithLabels("set")
                                .Observe(elapsed);
                        }
                    }
                    break;
                case "EasyCaching.WriteGetCacheBefore" when timestampFetcher.Fetch(payload) is long timestamp:
                    this.writeGetLocal.Value = timestamp;
                    break;
                case "EasyCaching.WriteGetCacheAfter" when timestampFetcher.Fetch(payload) is long timestamp:
                    {
                        var beforeTimeStamp = this.writeGetLocal.Value;

                        if (beforeTimeStamp > 0)
                        {
                            var elapsed = TimeSpan.FromTicks(timestamp - beforeTimeStamp).TotalSeconds;

                            PrometheusCounters.EasyCachingOperationDuration
                                .WithLabels("get")
                                .Observe(elapsed);
                        }
                    }
                    break;
            }
        }
    }
}
