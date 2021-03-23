using Prometheus.Contrib.EventListeners.Counters;

namespace Prometheus.Contrib.EventListeners.Adapters
{
    internal class PrometheusSqlClientCounterAdapter : BaseAdapter
    {
        public const string EventSourceName = "Microsoft.Data.SqlClient.EventSource";

        internal readonly MeanCounter ActiveHardConnections = new MeanCounter("active-hard-connections", "sqlclient_active_hard_connections_total", "Actual active connections are made to servers");
        internal readonly IncrementCounter HardConnects = new IncrementCounter("hard-connects", "sqlclient_hard_connects_per_second", "Actual connections are made to servers");
        internal readonly IncrementCounter HardDisconnects = new IncrementCounter("hard-disconnects", "sqlclient_hard_disconnects_per_second", "Actual disconnections are made to servers");
        internal readonly MeanCounter ActiveSoftConnections = new MeanCounter("active-soft-connects", "sqlclient_active_soft_connections_total", "Active connections got from connection pool");
        internal readonly IncrementCounter SoftConnects = new IncrementCounter("soft-connects", "sqlclient_soft_connects_per_second", "Connections got from connection pool");
        internal readonly IncrementCounter SoftDisconnects = new IncrementCounter("soft-disconnects", "sqlclient_soft_disconnects_per_second", "Connections returned to the connection pool");
        internal readonly MeanCounter NumberOfNonPooledConnections = new MeanCounter("number-of-non-pooled-connections", "sqlclient_non_pooled_connections_total", "Number of connections are not using connection pooling");
        internal readonly MeanCounter NumberOfPooledConnections = new MeanCounter("number-of-pooled-connections", "sqlclient_pooled_connections_total", "Number of connections are managed by connection pooler");
        internal readonly MeanCounter NumberOfActiveConnectionPoolGroups = new MeanCounter("number-of-active-connection-pool-groups", "sqlclient_active_connection_pool_groups_total", "Number of active unique connection strings");
        internal readonly MeanCounter NumberOfInactiveConnectionPoolGroups = new MeanCounter("number-of-inactive-connection-pool-groups", "sqlclient_inactive_connection_pool_groups_total", "Number of unique connection strings waiting for pruning");
        internal readonly MeanCounter NumberOfActiveConnectionPools = new MeanCounter("number-of-active-connection-pools", "sqlclient_active_connection_pools_total", "Number of active connection pools");
        internal readonly MeanCounter NumberOfInactiveConnectionPools = new MeanCounter("number-of-inactive-connection-pools", "sqlclient_inactive_connection_pools_total", "Number of inactive connection pools");
        internal readonly MeanCounter NumberOfActiveConnections = new MeanCounter("number-of-active-connections", "sqlclient_active_connections_total", "Number of active connections");
        internal readonly MeanCounter NumberOfFreeConnections = new MeanCounter("number-of-free-connections", "sqlclient_free_connections_total", "Number of free-ready connections");
        internal readonly MeanCounter NumberOfStasisConnections = new MeanCounter("number-of-stasis-connections", "sqlclient_stasis_connections_total", "Number of connections currently waiting to be ready");
        internal readonly IncrementCounter NumberOfReclaimedConnections = new IncrementCounter("number-of-reclaimed-connections", "sqlclient_reclaimed_connections_total", "Number of reclaimed connections from GC");
    }
}
