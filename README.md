# prometheus-net-contrib

[![Build status](https://dev.azure.com/alexvaluiskyi/metrics/_apis/build/status/alexvaluyskiy.prometheus-net-contrib)](https://dev.azure.com/alexvaluiskyi/metrics/_build/latest?definitionId=13) [![Nuget](https://img.shields.io/nuget/v/prometheus-net.Contrib.svg)](https://www.nuget.org/packages/prometheus-net.Contrib/)

A plugin for the prometheus-net package, exposing event counters and diagnostic listeners for .NET Core Runtime, ASP.NET Core, SignalR, GRPC, etc.

## Installation
Supports .NET core v3.0+ only.

Add the package from [nuget](https://www.nuget.org/packages/prometheus-net.Contrib):
```powershell
dotnet add package prometheus-net.Contrib
```

And then start the collectors:
```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        ...
        services.AddPrometheusCounters();
        services.AddPrometheusAspNetCoreMetrics();
        services.AddPrometheusHttpClientMetrics();
        services.AddPrometheusSqlClientMetrics();
    }
}
```

## .NET Core 3.0 Event Counters

### Runtime
| Name | Type | Description |
|--|--|--|
| runtime_cpu_usage_ratio | Gauge  | CPU usage in percents  |
| runtime_memory_working_set_megabytes | Gauge  | Working Set in megabytes  |
| runtime_gc_heap_size_megabytes | Gauge  | GC Heap Size in megabytes  |
| runtime_gc_count | Gauge  | GC Count  |
| runtime_exceptions_total | Gauge  | Exception Count  |
| runtime_threadpool_threads_total | Gauge  | ThreadPool Thread Count  |
| runtime_threadpool_queue_length | Gauge  | Monitor Lock Contention Count  |
| runtime_threadpool_completed_items_total | Gauge  | ThreadPool Queue Length  |
| runtime_time_in_gc | Gauge  | ThreadPool Completed Work Item Count  |
| runtime_gc_size_bytes | Gauge  | GC size in bytes  |
| runtime_allocation_rate_bytes | Gauge  | Allocation Rate in bytes  |
| runtime_assemblies_total | Gauge  | Number of Assemblies Loaded  |
| runtime_active_timers_total | Gauge  | Number of Active Timers  |

### ASP .NET Core

| Name | Type | Description |
|--|--|--|
| aspnetcore_requests_per_second | Gauge  | Request Rate  |
| aspnetcore_requests_total | Gauge  | Total Requests  |
| aspnetcore_requests_current_total | Gauge  | Current Requests  |
| aspnetcore_requests_failed_total | Gauge  | Failed Requests  |

### ASP .NET Core SignalR

| Name | Type | Description |
|--|--|--|
| signalr_connections_started_total  | Gauge  | Total Connections Started  |
| signalr_connections_stopped_total | Gauge  | Total Connections Stopped  |
| signalr_connections_timed_out_total | Gauge  | Total Connections Timed Out  |
| signalr_connections_current_total | Gauge  | Current Connections  |
| signalr_connections_duration_seconds | Gauge  | Average Connection Duration |

### ASP .NET Core GRPC Server

| Name | Type | Description |
|--|--|--|
| grpc_server_calls_total  | Gauge  | Total Calls  |
| grpc_server_calls_current_total | Gauge  | Current Calls  |
| grpc_server_calls_failed_total | Gauge  | Total Calls Failed  |
| grpc_server_deadline_exceeded_total | Gauge  | Total Calls Deadline Exceeded |
| grpc_server_messages_sent_total | Gauge  | Total Messages Sent |
| grpc_server_messages_received_total | Gauge  | Total Messages Received |
| grpc_server_calls_unimplemented_total | Gauge  | Total Calls Unimplemented |

### ASP .NET Core GRPC Client

| Name | Type | Description |
|--|--|--|
| grpc_client_calls_total  | Gauge  | Total Calls  |
| grpc_client_calls_current_total | Gauge  | Current Calls  |
| grpc_client_calls_failed_total | Gauge  | Total Calls Failed  |
| grpc_client_calls_deadline_exceeded_total | Gauge  | Total Calls Deadline Exceeded |
| grpc_client_messages_sent_total | Gauge  | Total Messages Sent |
| grpc_client_messages_received_total | Gauge  | Total Messages Received |

## .NET Core Diagnostic Listeners
### ASP .NET Core

| Name | Type | Description |
|--|--|--|
| aspnetcore_requests_duration_seconds | Histogram  |  The duration of HTTP requests processed by an ASP.NET Core application  |
| aspnetcore_requests_errors_total  | Counter  | Total HTTP requests received errors  |

### HTTP Client

| Name | Type | Description |
|--|--|--|
| http_client_requests_duration_seconds | Histogram  | Time between first byte of request headers sent to last byte of response received |
| http_client_requests_errors_total  | Counter  | Total HTTP requests sent errors  |

### SQL Client

| Name | Type | Description |
|--|--|--|
| sqlclient_commands_duration_seconds | Histogram  | The duration of DB requests processed by an application |
| sqlclient_commands_errors_total  | Counter  | Total DB requests errors  |
| sqlclient_connections_total | Counter  | Total DB connections |
| sqlclient_connections_errors  | Counter  | Total DB connections errors  |
| sqlclient_transactions_committed_total | Counter  | Total committed transactions |
| sqlclient_transactions_rollback_total  | Counter  | Total HTTP requests sent errors  |
| sqlclient_received_buffers  | Gauge  | 	Returns the number of tabular data stream (TDS) packets received by the provider from SQL Server  |
| sqlclient_sent_buffers  | Gauge  | Returns the number of TDS packets sent to SQL Server   |
| sqlclient_received_bytes  | Gauge  | Returns the number of bytes of data in the TDS packets received by the provider from SQL Server  |
| sqlclient_sent_bytes  | Gauge  | Returns the number of bytes of data sent to SQL Server in TDS packets  |
| sqlclient_connection_time  | Gauge  | The amount of time (in milliseconds) that the connection has been opened  |
| sqlclient_cursor_open  | Gauge  | Returns the number of times a cursor was open through the connection |
| sqlclient_execution_time  | Gauge  |   |
| sqlclient_idu_count  | Gauge  | Returns the total number of INSERT, DELETE, and UPDATE statements executed through the connection  |
| sqlclient_idu_rows  | Gauge  | Returns the total number of rows affected by INSERT, DELETE, and UPDATE statements executed through the connection  |
| sqlclient_network_server_time  | Gauge  | Returns the cumulative amount of time (in milliseconds) that the provider spent waiting for replies from the server  |
| sqlclient_prepared_exec  | Gauge  | Returns the number of prepared commands executed through the connection  |
| sqlclient_prepares  | Gauge  | Returns the number of statements prepared through the connection  |
| sqlclient_select_count  | Gauge  | Returns the number of SELECT statements executed through the connection  |
| sqlclient_select_rows  | Gauge  | Returns the number of rows selected  |
| sqlclient_server_roundtrips  | Gauge  | Returns the number of times the connection sent commands to the server and got a reply back  |
| sqlclient_sum_result_sets  | Gauge  | Returns the number of result sets  |
| sqlclient_transacions  | Gauge  | Returns the number of user transactions  |
| sqlclient_unprepared_exec  | Gauge  | Returns the number of unprepared statements executed through the connection  |

### GRPC Client
| Name | Type | Description |
|--|--|--|
| grpc_client_requests_duration_seconds | Histogram  | Time between first byte of request headers sent to last byte of response received |
| grpc_client_requests_errors_total  | Counter  | Total GRPC requests sent errors  |

### Identity Server
| Name | Type | Description |
|--|--|--|
| idsrv_client_authentication_success_total | Counter | Gets raised for successful client authentication at the token endpoint |
| idsrv_client_authentication_failure_total  | Counter  | Gets raised for failed client authentication at the token endpoint  |
| idsrv_token_issued_success_total | Counter  | Gets raised for successful attempts to request access tokens |
| idsrv_token_issued_failure_total  | Counter  | Gets raised for failed attempts to request access tokens  |
| idsrv_user_login_success_total | Counter  | Gets raised by the UI for successful user logins |
| idsrv_user_login_failure_total | Counter  | Gets raised by the UI for failed user logins |
| idsrv_unhandled_exceptions_total | Counter  | Gets raised for unhandled exceptions |