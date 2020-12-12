using System.Collections.Generic;
using Prometheus.Contrib.Core;
using Prometheus.Contrib.EventListeners.Counters;

namespace Prometheus.Contrib.EventListeners.Adapters
{
    internal class PrometheusRuntimeCounterAdapter : ICounterAdapter
    {
        public const string EventSourceName = "System.Runtime";

        internal MeanCounter RuntimeCpuUsage = new MeanCounter("cpu-usage", "runtime_cpu_usage_ratio", "CPU usage in percents");
        internal MeanCounter RuntimeWorkingSet = new MeanCounter("working-set","runtime_memory_working_set_megabytes", "Working Set in megabytes");
        internal MeanCounter RuntimeGcHeapSize = new MeanCounter("gc-heap-size", "runtime_gc_heap_size_megabytes", "GC Heap Size in megabytes");
        
        // TODO: cover other generations
        internal IncrementCounter RuntimeGcCount = new IncrementCounter("gen-0-gc-count", "runtime_gc_count", "GC Count");
        
        internal MeanCounter RuntimeThreadPoolThreadCount = new MeanCounter("threadpool-thread-count", "runtime_threadpool_threads_total", "ThreadPool Thread Count");
        internal IncrementCounter MonitorLockContentionCount = new IncrementCounter("monitor-lock-contention-count", "runtime_lock_contention_total", "Monitor Lock Contention Count");
        internal MeanCounter RuntimeThreadPoolQueueLength = new MeanCounter("threadpool-queue-length", "runtime_threadpool_queue_length", "ThreadPool Queue Length");
        internal IncrementCounter RuntimeThreadPoolCompletedItemsCount = new IncrementCounter("threadpool-completed-items-count","runtime_threadpool_completed_items_total", "ThreadPool Completed Work Item Count");
        internal IncrementCounter RuntimeAllocRate = new IncrementCounter("alloc-rate", "runtime_allocation_rate_bytes", "Allocation Rate in bytes");
        internal MeanCounter RuntimeActiveTimerCount = new MeanCounter("active-timer-count", "runtime_active_timers_total", "Number of Active Timers");
        internal MeanCounter RuntimeGcFragmentation = new MeanCounter("gc-fragmentation", "runtime_gc_fragmentation_ratio", "GC Fragmentation");
        internal IncrementCounter RuntimeExceptionCount = new IncrementCounter("exception-count","runtime_exceptions_total", "Exception Count");
        internal MeanCounter RuntimeTimeInGc = new MeanCounter("time-in-gc", "runtime_time_in_gc_ratio", "% Time in GC since last GC");
        
        // TODO: cover other generations
        internal MeanCounter RuntimeGcSize = new MeanCounter("gen-0-size", "runtime_gc_size_bytes", "GC size in bytes");
        
        internal MeanCounter RuntimeAssemblyCount = new MeanCounter("assembly-count", "runtime_assemblies_total", "Number of Assemblies Loaded");
        internal MeanCounter RuntimeIlBytesJitted = new MeanCounter("il-bytes-jitted", "runtime_il_jitted_bytes", "IL Bytes Jitted");
        internal MeanCounter RuntimeMethodsJittedCount = new MeanCounter("methods-jitted-count", "runtime_methods_jitted_total", "Number of Methods Jitted");

        private readonly Dictionary<string, BaseCounter> _counters;

        private static class PrometheusCounters
        {
            public static Gauge RuntimeGcCount = Metrics.CreateGauge("runtime_gc_count", "GC Count", new GaugeConfiguration { LabelNames = new[] { "gen" } });
            public static Gauge RuntimeGcSize = Metrics.CreateGauge("runtime_gc_size_bytes", "GC size in bytes", new GaugeConfiguration { LabelNames = new[] { "gen" } });
        }
        
        public PrometheusRuntimeCounterAdapter()
        {
            _counters = CounterUtils.GenerateDictionary(this);
        }

        public void OnCounterEvent(IDictionary<string, object> eventPayload)
        {
            if (!eventPayload.TryGetValue("Name", out var counterName))
            {
                return;
            }
            
            if (!_counters.TryGetValue((string) counterName, out var counter))
                return;

            counter.TryReadEventCounterData(eventPayload);
        }
    }
}
