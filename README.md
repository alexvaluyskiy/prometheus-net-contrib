# prometheus-net-contrib
[![Nuget](https://img.shields.io/nuget/v/prometheus-net.Contrib.svg)](https://www.nuget.org/packages/prometheus-net.Contrib/) ![Nuget](https://img.shields.io/nuget/dt/prometheus-net.Contrib.svg)

A plugin for the prometheus-net package, exposing event counters and diagnostic listeners for .NET Core Runtime, ASP.NET Core, SignalR, etc.

## Installation
Supports .NET core v3.0+ only.

Add the package from [nuget](https://www.nuget.org/packages/prometheus-net.Contrib):
```powershell
dotnet add package prometheus-net.Contrib
```

And then start the collectors:
```csharp
services.AddPrometheusCounters();
services.AddPrometheusAspNetCoreMetrics();
services.AddPrometheusHttpClientMetrics();
```

## .NET Core 3.0 Event Counters

### Runtime
| Name | Type | Description |
|--|--|--|
| runtime_cpu_usage | Gauge  | CPU usage in percents  |
| runtime_memory_working_set | Gauge  | Working Set in megabytes  |
| runtime_gc_heap_size | Gauge  | GC Heap Size in megabytes  |
| runtime_gc_count | Gauge  | GC Count  |
| runtime_exception_count | Gauge  | Exception Count  |
| runtime_threadpool_thread_count | Gauge  | ThreadPool Thread Count  |
| runtime_threadpool_queue_length | Gauge  | Monitor Lock Contention Count  |
| runtime_threadpool_completed_items_count | Gauge  | ThreadPool Queue Length  |
| runtime_time_in_gc | Gauge  | ThreadPool Completed Work Item Count  |
| runtime_gc_size | Gauge  | GC size in bytes  |
| runtime_alloc_rate | Gauge  | Allocation Rate in bytes  |
| runtime_assembly_count | Gauge  | Number of Assemblies Loaded  |
| runtime_active_timer_count | Gauge  | Number of Active Timers  |

### ASP .NET Core

| Name | Type | Description |
|--|--|--|
| aspnetcore_counters_requests_per_second | Gauge  | Request Rate  |
| aspnetcore_counters_total_requests | Gauge  | Total Requests  |
| aspnetcore_counters_current_requests | Gauge  | Current Requests  |
| aspnetcore_counters_failed_requests | Gauge  | Failed Requests  |

### ASP .NET Core SignalR

| Name | Type | Description |
|--|--|--|
| signalr_counters_connections_started  | Gauge  | Total Connections Started  |
| signalr_counters_connections_stopped | Gauge  | Total Connections Stopped  |
| signalr_counters_connections_timed_out | Gauge  | Total Connections Timed Out  |
| signalr_counters_current_connections | Gauge  | Current Connections  |
| signalr_counters_connections_duration | Gauge  | Average Connection Duration |


