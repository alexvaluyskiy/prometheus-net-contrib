using Prometheus.Contrib.EventListeners.Counters;

namespace Prometheus.Contrib.EventListeners.Adapters
{
    internal class PrometheusRuntimeCounterAdapter : BaseAdapter
    {
        public const string EventSourceName = "System.Runtime";

        internal MeanCounter RuntimeCpuUsage = new MeanCounter("cpu-usage", "runtime_cpu_usage_ratio", "CPU usage in percents");
        internal MeanCounter RuntimeWorkingSet = new MeanCounter("working-set","runtime_memory_working_set_megabytes", "Working Set in megabytes");
        internal MeanCounter RuntimeGcHeapSize = new MeanCounter("gc-heap-size", "runtime_gc_heap_size_megabytes", "GC Heap Size in megabytes");
        internal IncrementCounter RuntimeGcCountGen0 = new IncrementCounter("gen-0-gc-count", "runtime_gc_gen0_count", "GC Count (Gen 0)");
        internal IncrementCounter RuntimeGcCountGen1 = new IncrementCounter("gen-1-gc-count", "runtime_gc_gen1_count", "GC Count (Gen 1)");
        internal IncrementCounter RuntimeGcCountGen2 = new IncrementCounter("gen-2-gc-count", "runtime_gc_gen2_count", "GC Count (Gen 2)");
        internal MeanCounter RuntimeThreadPoolThreadCount = new MeanCounter("threadpool-thread-count", "runtime_threadpool_threads_total", "ThreadPool Thread Count");
        internal IncrementCounter MonitorLockContentionCount = new IncrementCounter("monitor-lock-contention-count", "runtime_lock_contention_total", "Monitor Lock Contention Count");
        internal MeanCounter RuntimeThreadPoolQueueLength = new MeanCounter("threadpool-queue-length", "runtime_threadpool_queue_length", "ThreadPool Queue Length");
        internal IncrementCounter RuntimeThreadPoolCompletedItemsCount = new IncrementCounter("threadpool-completed-items-count","runtime_threadpool_completed_items_total", "ThreadPool Completed Work Item Count");
        internal IncrementCounter RuntimeAllocRate = new IncrementCounter("alloc-rate", "runtime_allocation_rate_bytes", "Allocation Rate in bytes");
        internal MeanCounter RuntimeActiveTimerCount = new MeanCounter("active-timer-count", "runtime_active_timers_total", "Number of Active Timers");
        internal MeanCounter RuntimeGcFragmentation = new MeanCounter("gc-fragmentation", "runtime_gc_fragmentation_ratio", "GC Fragmentation");
        internal IncrementCounter RuntimeExceptionCount = new IncrementCounter("exception-count","runtime_exceptions_total", "Exception Count");
        internal MeanCounter RuntimeTimeInGc = new MeanCounter("time-in-gc", "runtime_time_in_gc_ratio", "% Time in GC since last GC");
        internal MeanCounter RuntimeGcSizeGen0 = new MeanCounter("gen-0-size", "runtime_gc_size_gen0_bytes", "GC size in bytes (Gen 0)");
        internal MeanCounter RuntimeGcSizeGen1 = new MeanCounter("gen-1-size", "runtime_gc_size_gen1_bytes", "GC size in bytes (Gen 1)");
        internal MeanCounter RuntimeGcSizeGen2 = new MeanCounter("gen-2-size", "runtime_gc_size_gen2_bytes", "GC size in bytes (Gen 2)");
        internal MeanCounter RuntimeGcSizeLoh = new MeanCounter("loh-size", "runtime_gc_size_loh_bytes", "GC size in bytes (LOH)");
        internal MeanCounter RuntimeGcSizePoh = new MeanCounter("poh-size", "runtime_gc_size_poh_bytes", "GC size in bytes (POH)");
        internal MeanCounter RuntimeAssemblyCount = new MeanCounter("assembly-count", "runtime_assemblies_total", "Number of Assemblies Loaded");
        internal MeanCounter RuntimeIlBytesJitted = new MeanCounter("il-bytes-jitted", "runtime_il_jitted_bytes", "IL Bytes Jitted");
        internal MeanCounter RuntimeMethodsJittedCount = new MeanCounter("methods-jitted-count", "runtime_methods_jitted_total", "Number of Methods Jitted");
    }
}
