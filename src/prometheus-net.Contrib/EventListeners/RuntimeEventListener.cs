using Prometheus;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;

namespace Prometheus.Contrib.EventListeners
{
    public class RuntimeEventListener : EventListener
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
            public static Gauge RuntimeCpuUsage = Metrics.CreateGauge("runtime_cpu_usage", "CPU usage in percents");
            public static Gauge RuntimeWorkingSet = Metrics.CreateGauge("runtime_memory_working_set", "Working Set in megabytes");
            public static Gauge RuntimeGcHeapSize = Metrics.CreateGauge("runtime_gc_heap_size", "GC Heap Size in megabytes");
            public static Gauge RuntimeGcCount = Metrics.CreateGauge("runtime_gc_count", "GC Count", new GaugeConfiguration { LabelNames = new[] { "gen" } });
            public static Gauge RuntimeExceptionCount = Metrics.CreateGauge("runtime_exception_count", "Exception Count");
            public static Gauge RuntimeThreadPoolThreadCount = Metrics.CreateGauge("runtime_threadpool_thread_count", "ThreadPool Thread Count");
            public static Gauge RuntimeThreadPoolQueueLength = Metrics.CreateGauge("runtime_threadpool_queue_length", "Monitor Lock Contention Count");
            public static Gauge RuntimeThreadPoolCompletedItemsCount = Metrics.CreateGauge("runtime_threadpool_completed_items_count", "ThreadPool Queue Length");
            public static Gauge RuntimeTimeInGc = Metrics.CreateGauge("runtime_time_in_gc", "ThreadPool Completed Work Item Count");
            public static Gauge RuntimeGcSize = Metrics.CreateGauge("runtime_gc_size", "GC size in bytes", new GaugeConfiguration { LabelNames = new[] { "gen" } });
            public static Gauge RuntimeAllocRate = Metrics.CreateGauge("runtime_alloc_rate", "Allocation Rate in bytes");
            public static Gauge RuntimeAssemblyCount = Metrics.CreateGauge("runtime_assembly_count", "Number of Assemblies Loaded");
            public static Gauge RuntimeActiveTimerCount = Metrics.CreateGauge("runtime_active_timer_count", "Number of Active Timers");
        }

        private IDictionary<string, string> eventArguments;
        private const string EventSourceName = "System.Runtime";

        public RuntimeEventListener(TimeSpan interval)
        {
            eventArguments = new Dictionary<string, string>
            {
                { "EventCounterIntervalSec", interval.TotalSeconds.ToString() }
            };
        }

        protected override void OnEventSourceCreated(EventSource source)
        {
            if (source.Name.Equals(EventSourceName))
            {
                EnableEvents(source, EventLevel.LogAlways, EventKeywords.All, eventArguments);
            }
        }

        private (string Name, double Value) GetRelevantMetric(IDictionary<string, object> eventPayload)
        {
            string counterName = "";
            double counterValue = 0;

            foreach (KeyValuePair<string, object> payload in eventPayload)
            {
                string key = payload.Key;
                string val = payload.Value.ToString();

                if (key.Equals("Name"))
                {
                    counterName = val;
                }
                else if (key.Equals("Mean") || key.Equals("Increment"))
                {
                    counterValue = double.Parse(val);
                }
            }

            return (counterName, counterValue);
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            if (!(eventData.EventName.Equals("EventCounters") && eventData.EventSource.Name.Equals(EventSourceName)))
            {
                return;
            }

            foreach (var payload in eventData.Payload)
            {
                if (payload is IDictionary<string, object> eventPayload)
                {
                    var counterKV = GetRelevantMetric(eventPayload);
                    switch (counterKV.Name)
                    {
                        case EventCountersConstants.RuntimeCpuUsage:
                            PrometheusCounters.RuntimeCpuUsage.Set(counterKV.Value);
                            break;
                        case EventCountersConstants.RuntimeWorkingSet:
                            PrometheusCounters.RuntimeWorkingSet.Set(counterKV.Value);
                            break;
                        case EventCountersConstants.RuntimeGcHeapSize:
                            PrometheusCounters.RuntimeGcHeapSize.Set(counterKV.Value);
                            break;
                        case EventCountersConstants.RuntimeGen0GcCount:
                            PrometheusCounters.RuntimeGcCount.WithLabels("0").Set(counterKV.Value);
                            break;
                        case EventCountersConstants.RuntimeGen1GcCount:
                            PrometheusCounters.RuntimeGcCount.WithLabels("1").Set(counterKV.Value);
                            break;
                        case EventCountersConstants.RuntimeGen2GcCount:
                            PrometheusCounters.RuntimeGcCount.WithLabels("2").Set(counterKV.Value);
                            break;
                        case EventCountersConstants.RuntimeExceptionCount:
                            PrometheusCounters.RuntimeExceptionCount.Set(counterKV.Value);
                            break;
                        case EventCountersConstants.RuntimeThreadPoolThreadCount:
                            PrometheusCounters.RuntimeThreadPoolThreadCount.Set(counterKV.Value);
                            break;
                        case EventCountersConstants.RuntimeThreadPoolQueueLength:
                            PrometheusCounters.RuntimeThreadPoolQueueLength.Set(counterKV.Value);
                            break;
                        case EventCountersConstants.RuntimeThreadPoolCompletedItemsCount:
                            PrometheusCounters.RuntimeThreadPoolCompletedItemsCount.Set(counterKV.Value);
                            break;
                        case EventCountersConstants.RuntimeTimeInGc:
                            PrometheusCounters.RuntimeTimeInGc.Set(counterKV.Value);
                            break;
                        case EventCountersConstants.RuntimeGen0Size:
                            PrometheusCounters.RuntimeGcSize.WithLabels("0").Set(counterKV.Value);
                            break;
                        case EventCountersConstants.RuntimeGen1Size:
                            PrometheusCounters.RuntimeGcSize.WithLabels("1").Set(counterKV.Value);
                            break;
                        case EventCountersConstants.RuntimeGen2Size:
                            PrometheusCounters.RuntimeGcSize.WithLabels("2").Set(counterKV.Value);
                            break;
                        case EventCountersConstants.RuntimeLohSize:
                            PrometheusCounters.RuntimeGcSize.WithLabels("loh").Set(counterKV.Value);
                            break;
                        case EventCountersConstants.RuntimeAllocRate:
                            PrometheusCounters.RuntimeAllocRate.Set(counterKV.Value);
                            break;
                        case EventCountersConstants.RuntimeAssemblyCount:
                            PrometheusCounters.RuntimeAssemblyCount.Set(counterKV.Value);
                            break;
                        case EventCountersConstants.RuntimeActiveTimerCount:
                            PrometheusCounters.RuntimeActiveTimerCount.Set(counterKV.Value);
                            break;
                    }
                }
            }
        }
    }
}
