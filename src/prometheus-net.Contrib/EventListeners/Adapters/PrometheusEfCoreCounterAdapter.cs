using Prometheus.Contrib.Core;

namespace Prometheus.Contrib.EventListeners.Adapters
{
    public class PrometheusEfCoreCounterAdapter : ICounterAdapter
    {
        private static class EfCoreCountersConstants
        {
            public const string EfCoreActiveDbContexts = "active-db-contexts";
            public const string EfCoreTotalQueries = "total-queries";
            public const string EfCoreQueriesPerSecond = "queries-per-second";
            public const string EfCoreTotalSaveChanges = "total-save-changes";
            public const string EfCoreSaveChangesPerSecond = "save-changes-per-second";
            public const string EfCoreCompiledQueryCacheHitRate = "compiled-query-cache-hit-rate";
            public const string EfCoreTotalExecutionStrategyOperationFailures = "total-execution-strategy-operation-failures";
            public const string EfCoreExecutionStrategyOperationFailuresPerSecond = "execution-strategy-operation-failures-per-second";
            public const string EfCoreTotalOptimisticConcurrencyFailures = "total-optimistic-concurrency-failures";
            public const string EfCoreOptimisticConcurrencyFailuresPerSecond = "optimistic-concurrency-failures-per-second";
        }

        private static class EfCorePrometheusCounters
        {
            public static Gauge EfCoreActiveDbContexts = Metrics.CreateGauge("efcore_active_dbcontexts_total", "Active DbContexts");
            public static Gauge EfCoreTotalQueries = Metrics.CreateGauge("efcore_queries_total", "Queries (Total)");
            public static Gauge EfCoreQueriesPerSecond = Metrics.CreateGauge("efcore_queries_per_second", "Queries");
            public static Gauge EfCoreTotalSaveChanges = Metrics.CreateGauge("efcore_savechanges_total", "SaveChanges (Total)");
            public static Gauge EfCoreSaveChangesPerSecond = Metrics.CreateGauge("efcore_savechanges_per_second", "SaveChanges");
            public static Gauge EfCoreCompiledQueryCacheHitRate = Metrics.CreateGauge("efcore_compiled_query_cache_hit_ratio", "Query Cache Hit Rate");
            public static Gauge EfCoreTotalExecutionStrategyOperationFailures = Metrics.CreateGauge("efcore_execution_strategy_operation_failures_total", "Execution Strategy Operation Failures (Total)");
            public static Gauge EfCoreExecutionStrategyOperationFailuresPerSecond = Metrics.CreateGauge("efcore_execution_strategy_operation_failures_per_second", "Execution Strategy Operation Failures");
            public static Gauge EfCoreTotalOptimisticConcurrencyFailures = Metrics.CreateGauge("efcore_optimistic_concurrency_failures_total", "Optimistic Concurrency Failures (Total)");
            public static Gauge EfCoreOptimisticConcurrencyFailuresPerSecond = Metrics.CreateGauge("efcore_optimistic_concurrency_failures_per_second", "Optimistic Concurrency Failures");
        }

        public void OnCounterEvent(string name, double value)
        {
            switch (name)
            {
                case EfCoreCountersConstants.EfCoreActiveDbContexts:
                    EfCorePrometheusCounters.EfCoreActiveDbContexts.Set(value);
                    break;
                case EfCoreCountersConstants.EfCoreTotalQueries:
                    EfCorePrometheusCounters.EfCoreTotalQueries.Set(value);
                    break;
                case EfCoreCountersConstants.EfCoreQueriesPerSecond:
                    EfCorePrometheusCounters.EfCoreQueriesPerSecond.Set(value);
                    break;
                case EfCoreCountersConstants.EfCoreTotalSaveChanges:
                    EfCorePrometheusCounters.EfCoreTotalSaveChanges.Set(value);
                    break;
                case EfCoreCountersConstants.EfCoreSaveChangesPerSecond:
                    EfCorePrometheusCounters.EfCoreSaveChangesPerSecond.Set(value);
                    break;
                case EfCoreCountersConstants.EfCoreCompiledQueryCacheHitRate:
                    EfCorePrometheusCounters.EfCoreCompiledQueryCacheHitRate.Set(value);
                    break;
                case EfCoreCountersConstants.EfCoreTotalExecutionStrategyOperationFailures:
                    EfCorePrometheusCounters.EfCoreTotalExecutionStrategyOperationFailures.Set(value);
                    break;
                case EfCoreCountersConstants.EfCoreExecutionStrategyOperationFailuresPerSecond:
                    EfCorePrometheusCounters.EfCoreExecutionStrategyOperationFailuresPerSecond.Set(value);
                    break;
                case EfCoreCountersConstants.EfCoreTotalOptimisticConcurrencyFailures:
                    EfCorePrometheusCounters.EfCoreTotalOptimisticConcurrencyFailures.Set(value);
                    break;
                case EfCoreCountersConstants.EfCoreOptimisticConcurrencyFailuresPerSecond:
                    EfCorePrometheusCounters.EfCoreOptimisticConcurrencyFailuresPerSecond.Set(value);
                    break;
            }
        }
    }
}
