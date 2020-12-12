using System.Collections.Generic;
using Prometheus.Contrib.Core;
using Prometheus.Contrib.EventListeners.Counters;

namespace Prometheus.Contrib.EventListeners.Adapters
{
    internal class PrometheusEfCoreCounterAdapter : ICounterAdapter
    {
        public const string EventSourceName = "Microsoft.EntityFrameworkCore";
        
        internal readonly MeanCounter ActiveDbContexts = new MeanCounter("active-db-contexts", "efcore_active_dbcontexts_total", "Active DbContexts");
        internal readonly MeanCounter TotalQueries = new MeanCounter("total-queries", "efcore_queries_total", "Queries (Total)");
        internal readonly IncrementCounter QueriesPerSecond = new IncrementCounter("queries-per-second", "efcore_queries_per_second", "Queries");
        internal readonly MeanCounter TotalSaveChanges = new MeanCounter("total-save-changes", "efcore_savechanges_total", "SaveChanges (Total)");
        internal readonly IncrementCounter SaveChangesPerSecond = new IncrementCounter("save-changes-per-second", "efcore_savechanges_per_second", "SaveChanges");
        internal readonly MeanCounter CompiledQueryCacheHitRate = new MeanCounter("compiled-query-cache-hit-rate", "efcore_compiled_query_cache_hit_ratio", "Query Cache Hit Rate");
        internal readonly MeanCounter TotalExecutionStrategyOperationFailures = new MeanCounter("total-execution-strategy-operation-failures", "efcore_execution_strategy_operation_failures_total", "Execution Strategy Operation Failures (Total)");
        internal readonly IncrementCounter ExecutionStrategyOperationFailuresPerSecond = new IncrementCounter("execution-strategy-operation-failures-per-second", "efcore_execution_strategy_operation_failures_per_second", "Execution Strategy Operation Failures");
        internal readonly MeanCounter TotalOptimisticConcurrencyFailures = new MeanCounter("total-optimistic-concurrency-failures", "efcore_optimistic_concurrency_failures_total", "Optimistic Concurrency Failures (Total)");
        internal readonly IncrementCounter OptimisticConcurrencyFailuresPerSecond = new IncrementCounter("optimistic-concurrency-failures-per-second", "efcore_optimistic_concurrency_failures_per_second", "Optimistic Concurrency Failures");
        
        private readonly Dictionary<string, BaseCounter> _counters;

        public PrometheusEfCoreCounterAdapter()
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
