using Prometheus.Contrib.Core;

namespace Prometheus.Contrib.EventListeners.Adapters
{
    public class PrometheusRuntimeCounterAdapter : ICounterAdapter
    {
        private static class EventCountersConstants
        {
            public const string RuntimeCpuUsage = "cpu-usage";
            public const string RuntimeWorkingSet = "working-set";
            public const string RuntimeGcHeapSize = "gc-heap-size";
            public const string RuntimeGen0GcCount = "gen-0-gc-count";
            public const string RuntimeGen1GcCount = "gen-1-gc-count";
            public const string RuntimeGen2GcCount = "gen-2-gc-count";
            public const string RuntimeExceptionCount = "exception-count";
            public const string RuntimeThreadPoolThreadCount = "threadpool-thread-count";
            public const string MonitorLockContentionCount = "monitor-lock-contention-count";
            public const string RuntimeThreadPoolQueueLength = "threadpool-queue-length";
            public const string RuntimeThreadPoolCompletedItemsCount = "threadpool-completed-items-count";
            public const string RuntimeTimeInGc = "time-in-gc";
            public const string RuntimeGen0Size = "gen-0-size";
            public const string RuntimeGen1Size = "gen-1-size";
            public const string RuntimeGen2Size = "gen-2-size";
            public const string RuntimeLohSize = "loh-size";
            public const string RuntimeAllocRate = "alloc-rate";
            public const string RuntimeAssemblyCount = "assembly-count";
            public const string RuntimeActiveTimerCount = "active-timer-count";
        }

        private static class PrometheusCounters
        {
            public static Gauge RuntimeCpuUsage = Metrics.CreateGauge("runtime_cpu_usage_ratio", "CPU usage in percents");
            public static Gauge RuntimeWorkingSet = Metrics.CreateGauge("runtime_memory_working_set_megabytes", "Working Set in megabytes");
            public static Gauge RuntimeGcHeapSize = Metrics.CreateGauge("runtime_gc_heap_size_megabytes", "GC Heap Size in megabytes");
            public static Gauge RuntimeGcCount = Metrics.CreateGauge("runtime_gc_count", "GC Count", new GaugeConfiguration { LabelNames = new[] { "gen" } });
            public static Gauge RuntimeExceptionCount = Metrics.CreateGauge("runtime_exceptions_total", "Exception Count");
            public static Gauge RuntimeThreadPoolThreadCount = Metrics.CreateGauge("runtime_threadpool_threads_total", "ThreadPool Thread Count");
            public static Gauge MonitorLockContentionCount = Metrics.CreateGauge("runtime_lock_contention_total", "Monitor Lock Contention Count");
            public static Gauge RuntimeThreadPoolQueueLength = Metrics.CreateGauge("runtime_threadpool_queue_length", "ThreadPool Queue Length");
            public static Gauge RuntimeThreadPoolCompletedItemsCount = Metrics.CreateGauge("runtime_threadpool_completed_items_total", "ThreadPool Completed Work Item Count");
            public static Gauge RuntimeTimeInGc = Metrics.CreateGauge("runtime_time_in_gc_ratio", "% Time in GC since last GC");
            public static Gauge RuntimeGcSize = Metrics.CreateGauge("runtime_gc_size_bytes", "GC size in bytes", new GaugeConfiguration { LabelNames = new[] { "gen" } });
            public static Gauge RuntimeAllocRate = Metrics.CreateGauge("runtime_allocation_rate_bytes", "Allocation Rate in bytes");
            public static Gauge RuntimeAssemblyCount = Metrics.CreateGauge("runtime_assemblies_total", "Number of Assemblies Loaded");
            public static Gauge RuntimeActiveTimerCount = Metrics.CreateGauge("runtime_active_timers_total", "Number of Active Timers");
        }

        public void OnCounterEvent(string name, double value)
        {
            switch (name)
            {
                case EventCountersConstants.RuntimeCpuUsage:
                    PrometheusCounters.RuntimeCpuUsage.Set(value);
                    break;
                case EventCountersConstants.RuntimeWorkingSet:
                    PrometheusCounters.RuntimeWorkingSet.Set(value);
                    break;
                case EventCountersConstants.RuntimeGcHeapSize:
                    PrometheusCounters.RuntimeGcHeapSize.Set(value);
                    break;
                case EventCountersConstants.RuntimeGen0GcCount:
                    PrometheusCounters.RuntimeGcCount.WithLabels("0").Set(value);
                    break;
                case EventCountersConstants.RuntimeGen1GcCount:
                    PrometheusCounters.RuntimeGcCount.WithLabels("1").Set(value);
                    break;
                case EventCountersConstants.RuntimeGen2GcCount:
                    PrometheusCounters.RuntimeGcCount.WithLabels("2").Set(value);
                    break;
                case EventCountersConstants.RuntimeExceptionCount:
                    PrometheusCounters.RuntimeExceptionCount.Set(value);
                    break;
                case EventCountersConstants.RuntimeThreadPoolThreadCount:
                    PrometheusCounters.RuntimeThreadPoolThreadCount.Set(value);
                    break;
                case EventCountersConstants.MonitorLockContentionCount:
                    PrometheusCounters.MonitorLockContentionCount.Set(value);
                    break;
                case EventCountersConstants.RuntimeThreadPoolQueueLength:
                    PrometheusCounters.RuntimeThreadPoolQueueLength.Set(value);
                    break;
                case EventCountersConstants.RuntimeThreadPoolCompletedItemsCount:
                    PrometheusCounters.RuntimeThreadPoolCompletedItemsCount.Set(value);
                    break;
                case EventCountersConstants.RuntimeTimeInGc:
                    PrometheusCounters.RuntimeTimeInGc.Set(value);
                    break;
                case EventCountersConstants.RuntimeGen0Size:
                    PrometheusCounters.RuntimeGcSize.WithLabels("0").Set(value);
                    break;
                case EventCountersConstants.RuntimeGen1Size:
                    PrometheusCounters.RuntimeGcSize.WithLabels("1").Set(value);
                    break;
                case EventCountersConstants.RuntimeGen2Size:
                    PrometheusCounters.RuntimeGcSize.WithLabels("2").Set(value);
                    break;
                case EventCountersConstants.RuntimeLohSize:
                    PrometheusCounters.RuntimeGcSize.WithLabels("loh").Set(value);
                    break;
                case EventCountersConstants.RuntimeAllocRate:
                    PrometheusCounters.RuntimeAllocRate.Set(value);
                    break;
                case EventCountersConstants.RuntimeAssemblyCount:
                    PrometheusCounters.RuntimeAssemblyCount.Set(value);
                    break;
                case EventCountersConstants.RuntimeActiveTimerCount:
                    PrometheusCounters.RuntimeActiveTimerCount.Set(value);
                    break;
            }
        }
    }
}
