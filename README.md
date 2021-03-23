# prometheus-net-contrib

![Build status](https://github.com/alexvaluyskiy/prometheus-net-contrib/workflows/run-tests/badge.svg) [![Nuget](https://img.shields.io/nuget/v/prometheus-net.Contrib.svg)](https://www.nuget.org/packages/prometheus-net.Contrib/)

A plugin for the [prometheus-net](https://github.com/prometheus-net/prometheus-net) package, exposing event counters and diagnostic listeners for .NET Core Runtime, ASP.NET Core, SignalR, GRPC, etc.

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
| Name                                     | Type  | Description                          |
|------------------------------------------|-------|--------------------------------------|
| runtime_cpu_usage_ratio                  | Gauge | CPU usage in percents                |
| runtime_memory_working_set_megabytes     | Gauge | Working Set in megabytes             |
| runtime_gc_heap_size_megabytes           | Gauge | GC Heap Size in megabytes            |
| runtime_gc_count                         | Gauge | GC Count                             |
| runtime_exceptions_total                 | Gauge | Exception Count                      |
| runtime_threadpool_threads_total         | Gauge | ThreadPool Thread Count              |
| runtime_lock_contention_total            | Gauge | Monitor Lock Contention Count        |
| runtime_threadpool_queue_length          | Gauge | ThreadPool Queue Length              |
| runtime_threadpool_completed_items_total | Gauge | ThreadPool Completed Work Item Count |
| runtime_time_in_gc_ratio                 | Gauge | % Time in GC since last GC           |
| runtime_gc_size_bytes                    | Gauge | GC size in bytes                     |
| runtime_allocation_rate_bytes            | Gauge | Allocation Rate in bytes             |
| runtime_assemblies_total                 | Gauge | Number of Assemblies Loaded          |
| runtime_active_timers_total              | Gauge | Number of Active Timers              |

### ASP .NET Core

| Name                              | Type  | Description      |
|-----------------------------------|-------|------------------|
| aspnetcore_requests_per_second    | Gauge | Request Rate     |
| aspnetcore_requests_total         | Gauge | Total Requests   |
| aspnetcore_requests_current_total | Gauge | Current Requests |
| aspnetcore_requests_failed_total  | Gauge | Failed Requests  |

### ASP .NET Core SignalR

| Name                                 | Type  | Description                 |
|--------------------------------------|-------|-----------------------------|
| signalr_connections_started_total    | Gauge | Total Connections Started   |
| signalr_connections_stopped_total    | Gauge | Total Connections Stopped   |
| signalr_connections_timed_out_total  | Gauge | Total Connections Timed Out |
| signalr_connections_current_total    | Gauge | Current Connections         |
| signalr_connections_duration_seconds | Gauge | Average Connection Duration |

### ASP .NET Core GRPC Server

| Name                                  | Type  | Description                   |
|---------------------------------------|-------|-------------------------------|
| grpc_server_calls_total               | Gauge | Total Calls                   |
| grpc_server_calls_current_total       | Gauge | Current Calls                 |
| grpc_server_calls_failed_total        | Gauge | Total Calls Failed            |
| grpc_server_deadline_exceeded_total   | Gauge | Total Calls Deadline Exceeded |
| grpc_server_messages_sent_total       | Gauge | Total Messages Sent           |
| grpc_server_messages_received_total   | Gauge | Total Messages Received       |
| grpc_server_calls_unimplemented_total | Gauge | Total Calls Unimplemented     |

### ASP .NET Core GRPC Client

| Name                                      | Type  | Description                   |
|-------------------------------------------|-------|-------------------------------|
| grpc_client_calls_total                   | Gauge | Total Calls                   |
| grpc_client_calls_current_total           | Gauge | Current Calls                 |
| grpc_client_calls_failed_total            | Gauge | Total Calls Failed            |
| grpc_client_calls_deadline_exceeded_total | Gauge | Total Calls Deadline Exceeded |
| grpc_client_messages_sent_total           | Gauge | Total Messages Sent           |
| grpc_client_messages_received_total       | Gauge | Total Messages Received       |

## .NET Core Diagnostic Listeners
### ASP .NET Core

| Name                                 | Type      | Description                                                            |
|--------------------------------------|-----------|------------------------------------------------------------------------|
| aspnetcore_requests_duration_seconds | Histogram | The duration of HTTP requests processed by an ASP.NET Core application |
| aspnetcore_requests_errors_total     | Counter   | Total HTTP requests received errors                                    |

### HTTP Client

| Name                                  | Type      | Description                                                                       |
|---------------------------------------|-----------|-----------------------------------------------------------------------------------|
| http_client_requests_duration_seconds | Histogram | Time between first byte of request headers sent to last byte of response received |
| http_client_requests_errors_total     | Counter   | Total HTTP requests sent errors                                                   |

### SQL Client

| Name                                   | Type      | Description                                             |
|----------------------------------------|-----------|---------------------------------------------------------|
| sqlclient_commands_duration_seconds    | Histogram | The duration of DB requests processed by an application |
| sqlclient_commands_errors_total        | Counter   | Total DB command errors                                 |
| sqlclient_connections_opened_total     | Counter   | Total opened DB connections                             |
| sqlclient_connections_closed_total     | Counter   | Total closed DB connections                             |
| sqlclient_connections_errors_total     | Counter   | Total DB connections errors                             |
| sqlclient_transactions_committed_total | Counter   | Total committed transactions                            |
| sqlclient_transactions_rollback_total  | Counter   | Total rollback transactions                             |
| sqlclient_transactions_errors_total    | Counter   | Total DB transaction errors                             |

### SQL Client (Microsoft.Data.SqlClient 3.x)
| Name                                            | Type  | Description                                             |
|-------------------------------------------------|-------|---------------------------------------------------------|
| sqlclient_active_hard_connections_total         | Gauge | Actual active connections are made to servers           |
| sqlclient_hard_connects_per_second              | Gauge | Actual connections are made to servers                  |
| sqlclient_hard_disconnects_per_second           | Gauge | Actual disconnections are made to servers               |
| sqlclient_active_soft_connections_total         | Gauge | Active connections got from connection pool             |
| sqlclient_soft_connects_per_second              | Gauge | Connections got from connection pool                    |
| sqlclient_soft_disconnects_per_second           | Gauge | Connections returned to the connection pool             |
| sqlclient_non_pooled_connections_total          | Gauge | Number of connections are not using connection pooling  |
| sqlclient_pooled_connections_total              | Gauge | Number of connections are managed by connection pooler  |
| sqlclient_active_connection_pool_groups_total   | Gauge | Number of active unique connection strings              |
| sqlclient_inactive_connection_pool_groups_total | Gauge | Number of unique connection strings waiting for pruning |
| sqlclient_active_connection_pools_total         | Gauge | Number of active connection pools                       |
| sqlclient_inactive_connection_pools_total       | Gauge | Actual connections are made to servers                  |
| sqlclient_active_connections_total              | Gauge | Number of active connections                            |
| sqlclient_free_connections_total                | Gauge | Number of free-ready connections                        |
| sqlclient_stasis_connections_total              | Gauge | Number of connections currently waiting to be ready     |
| sqlclient_reclaimed_connections_total           | Gauge | Number of reclaimed connections from GC                 |

### GRPC Client
| Name                                  | Type      | Description                                                                       |
|---------------------------------------|-----------|-----------------------------------------------------------------------------------|
| grpc_client_requests_duration_seconds | Histogram | Time between first byte of request headers sent to last byte of response received |
| grpc_client_requests_errors_total     | Counter   | Total GRPC requests sent errors                                                   |

### Identity Server

```powershell
dotnet add package prometheus-net.IdentityServer
```

And then start the collectors:
```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddIdentityServer(options =>
        {
            options.Events.RaiseErrorEvents = true;
            options.Events.RaiseFailureEvents = true;
            options.Events.RaiseInformationEvents = true;
            options.Events.RaiseSuccessEvents = true;
        });

        services.AddPrometheusIdentityServerMetrics();
    }
}
```

| Name                                      | Type    | Description                                                                                                           |
|-------------------------------------------|---------|-----------------------------------------------------------------------------------------------------------------------|
| idsrv_api_authentication_failure_total    | Counter | Gets raised for successful API authentication at the introspection endpoint                                           |
| idsrv_api_authentication_failure_total    | Counter | Gets raised for failed API authentication at the introspection endpoint                                               |
| idsrv_client_authentication_success_total | Counter | Gets raised for successful client authentication at the token endpoint                                                |
| idsrv_client_authentication_failure_total | Counter | Gets raised for failed client authentication at the token endpoint                                                    |
| idsrv_token_issued_success_total          | Counter | Gets raised for successful attempts to request access tokens                                                          |
| idsrv_token_issued_failure_total          | Counter | Gets raised for failed attempts to request access tokens                                                              |
| idsrv_token_introspection_success_total   | Counter | Gets raised for successful attempts to request identity tokens, access tokens, refresh tokens and authorization codes |
| idsrv_token_introspection_failure_total   | Counter | Gets raised for failed attempts to request identity tokens, access tokens, refresh tokens and authorization codes     |
| idsrv_token_revoked_success_total         | Counter | Gets raised for successful token revocation requests.                                                                 |
| idsrv_user_login_success_total            | Counter | Gets raised by the UI for successful user logins                                                                      |
| idsrv_user_login_failure_total            | Counter | Gets raised by the UI for failed user logins                                                                          |
| idsrv_user_logout_success_total           | Counter | Gets raised for successful logout requests                                                                            |
| idsrv_consent_granted_total               | Counter | Gets raised in the consent UI                                                                                         |
| idsrv_consent_denied_total                | Counter | Gets raised in the consent UI                                                                                         |
| idsrv_unhandled_exceptions_total          | Counter | Gets raised for unhandled exceptions                                                                                  |
| idsrv_device_authorization_success_total  | Counter | Gets raised for successful device authorization requests                                                              |
| idsrv_device_authorization_success_total  | Counter | Gets raised for failed device authorization requests                                                                  |

## Prometheus healthchecks
It is possible to publish all healthchecks results to a prometheus
```csharp
public virtual void ConfigureServices(IServiceCollection services)
{
    services.AddHealthChecks()
        .AddSqlServer("<Connection String>", name: "sqlserver")

    services.AddSingleton<IHealthCheckPublisher, PrometheusHealthcheckPublisher>();

    return services;
}
```
