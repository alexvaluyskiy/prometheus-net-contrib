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

        private readonly PropertyFetcher<long> setBeforeOperationFetcher = new PropertyFetcher<long>("Timestamp");
        private readonly PropertyFetcher<long> setAfterOperationFetcher = new PropertyFetcher<long>("Timestamp");

        private readonly PropertyFetcher<long> getBeforeOperationFetcher = new PropertyFetcher<long>("Timestamp");
        private readonly PropertyFetcher<long> getAfterOperationFetcher = new PropertyFetcher<long>("Timestamp");

        private readonly PropertyFetcher<long> removeBeforeOperationFetcher = new PropertyFetcher<long>("Timestamp");
        private readonly PropertyFetcher<long> removeAfterOperationFetcher = new PropertyFetcher<long>("Timestamp");

        private readonly AsyncLocal<long> writeRemoveLocal = new AsyncLocal<long>();
        private readonly AsyncLocal<long> writeGetLocal = new AsyncLocal<long>();
        private readonly AsyncLocal<long> writeSetLocal = new AsyncLocal<long>();

        public EasyCachingListenerHandler(string sourceName) : base(sourceName)
        {
        }

        public override void OnCustom(string name, Activity activity, object payload)
        {
            switch (name)
            {
                case "EasyCaching.WriteSetCacheBefore":
                    {
                        if (setBeforeOperationFetcher.TryFetch(payload, out var timestampBefore))
                        {
                            this.writeSetLocal.Value = timestampBefore;
                        }
                    }
                    break;
                case "EasyCaching.WriteSetCacheAfter":
                    {
                        var timestampBefore = this.writeSetLocal.Value;

                        if (setAfterOperationFetcher.TryFetch(payload, out var timestampAfter))
                        {
                            var elapsed = TimeSpan.FromTicks(timestampAfter - timestampBefore).TotalSeconds;

                            if (elapsed == 0)
                            {
                                return;
                            }

                            PrometheusCounters.EasyCachingOperationDuration.WithLabels("set").Observe(elapsed);
                        }
                    }
                    break;
                case "EasyCaching.WriteGetCacheBefore":
                    {
                        if (getBeforeOperationFetcher.TryFetch(payload, out var timestampBefore))
                        {
                            this.writeGetLocal.Value = timestampBefore;
                        }
                    }
                    break;
                case "EasyCaching.WriteGetCacheAfter":
                    {
                        var timestampBefore = this.writeGetLocal.Value;

                        if (getAfterOperationFetcher.TryFetch(payload, out var timeStampAfter))
                        {
                            var elapsed = TimeSpan.FromTicks(timeStampAfter - timestampBefore).TotalSeconds;
                            
                            if (elapsed == 0)
                            {
                                return;
                            }

                            PrometheusCounters.EasyCachingOperationDuration.WithLabels("get").Observe(elapsed);
                        }
                    }
                    break;
                case "EasyCaching.WriteRemoveCacheBefore":
                    {
                        if (removeBeforeOperationFetcher.TryFetch(payload, out var timestampBefore))
                        {
                            this.writeGetLocal.Value = timestampBefore;
                        }
                    }
                    break;
                case "EasyCaching.WriteRemoveCacheAfter":
                    {
                        var timestampBefore = this.writeGetLocal.Value;

                        if (removeAfterOperationFetcher.TryFetch(payload, out var timeStampAfter))
                        {
                            var elapsed = TimeSpan.FromTicks(timeStampAfter - timestampBefore).TotalSeconds;

                            if (elapsed == 0)
                            {
                                return;
                            }

                            PrometheusCounters.EasyCachingOperationDuration.WithLabels("remove").Observe(elapsed);
                        }
                    }
                    break;
            }
        }
    }
}
