using System;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Prometheus.Contrib.Core;

namespace Prometheus.EntityFramework.Diagnostics
{
    public class EntityFrameworkListenerHandler : DiagnosticListenerHandler
    {
        private static class PrometheusCounters
        {
            public static readonly Histogram EfCoreCommandsDuration = Metrics.CreateHistogram(
                "efcore_command_duration_seconds",
                "The duration of DB requests processed by an application.",
                new HistogramConfiguration { Buckets = Histogram.ExponentialBuckets(0.001, 2, 16) });
            public static readonly Counter EfCoreCommandsErrors = Metrics.CreateCounter(
                "efcore_command_errors_total",
                "Total DB requests errors",
                new CounterConfiguration { LabelNames = new[] { "type" } });

            public static readonly Counter EfCoreConnectionsOpenTotal = Metrics.CreateCounter("efcore_connection_opened_total", "Total opened DB connections");
            public static readonly Counter EfCoreConnectionsCloseTotal = Metrics.CreateCounter("efcore_connection_closed_total", "Total closed DB connections");
            public static readonly Counter EfCoreConnectionsErrors = Metrics.CreateCounter("efcore_connection_errors_total", "Total DB connections errors");

            public static readonly Counter EfCoreTransactionsCommitedCount = Metrics.CreateCounter("efcore_transaction_committed_total", "Total committed transactions.");
            public static readonly Counter EfCoreTransactionsRollbackCount = Metrics.CreateCounter("efcore_transaction_rollback_total", "Total rollback transactions.");

            public static readonly Counter EfCoreDbContextCreatedCount = Metrics.CreateCounter("efcore_dbcontext_created_total", "Total created DBContexts");

            public static readonly Counter EfCoreQueryWarningsCount = Metrics.CreateCounter(
                "efcore_query_warnings_total", 
                "Total query warnings",
                new CounterConfiguration { LabelNames = new[] { "type" } });
        }

        public EntityFrameworkListenerHandler(string sourceName) : base(sourceName)
        {
        }

        public override void OnCustom(string name, Activity activity, object payload)
        {
            switch (name)
            {
                case "Microsoft.EntityFrameworkCore.Infrastructure.ContextInitialized":
                    PrometheusCounters.EfCoreDbContextCreatedCount.Inc();
                    break;
                case "Microsoft.EntityFrameworkCore.Infrastructure.ContextDisposed":
                    break;

                case "Microsoft.EntityFrameworkCore.Database.Connection.ConnectionOpening":
                    break;
                case "Microsoft.EntityFrameworkCore.Database.Connection.ConnectionOpened":
                    PrometheusCounters.EfCoreConnectionsOpenTotal.Inc();
                    break;
                case "Microsoft.EntityFrameworkCore.Database.Connection.ConnectionClosing":
                    break;
                case "Microsoft.EntityFrameworkCore.Database.Connection.ConnectionClosed":
                    PrometheusCounters.EfCoreConnectionsCloseTotal.Inc();
                    break;
                case "Microsoft.EntityFrameworkCore.Database.Connection.ConnectionError":
                    PrometheusCounters.EfCoreConnectionsErrors.Inc();
                    break;

                case "Microsoft.EntityFrameworkCore.Database.Command.CommandCreating":
                    break;
                case "Microsoft.EntityFrameworkCore.Database.Command.CommandCreated":
                    break;
                case "Microsoft.EntityFrameworkCore.Database.Command.CommandExecuting":
                    break;
                case "Microsoft.EntityFrameworkCore.Database.Command.CommandExecuted":
                    {
                        if (payload is CommandExecutedEventData commandExecuted)
                            PrometheusCounters.EfCoreCommandsDuration.Observe(commandExecuted.Duration.TotalSeconds);
                    }
                    break;
                case "Microsoft.EntityFrameworkCore.Database.Command.CommandError":
                    PrometheusCounters.EfCoreCommandsErrors.WithLabels("command").Inc();
                    break;

                case "Microsoft.EntityFrameworkCore.Database.Transaction.TransactionStarting":
                    break;
                case "Microsoft.EntityFrameworkCore.Database.Transaction.TransactionStarted":
                    break;
                case "Microsoft.EntityFrameworkCore.Database.Transaction.TransactionCommitting":
                    break;
                case "Microsoft.EntityFrameworkCore.Database.Transaction.TransactionCommitted":
                    if (payload is TransactionEndEventData _)
                        PrometheusCounters.EfCoreTransactionsCommitedCount.Inc();
                    break;
                case "Microsoft.EntityFrameworkCore.Database.Transaction.TransactionRollingBack":
                    break;
                case "Microsoft.EntityFrameworkCore.Database.Transaction.TransactionRolledBack":
                    if (payload is TransactionEndEventData _)
                        PrometheusCounters.EfCoreTransactionsRollbackCount.Inc();
                    break;
                case "Microsoft.EntityFrameworkCore.Database.Transaction.TransactionError":
                    if (payload is TransactionErrorEventData _)
                        PrometheusCounters.EfCoreCommandsErrors.WithLabels("transaction").Inc();
                    break;
                case "Microsoft.EntityFrameworkCore.Database.Transaction.TransactionUsed":
                    break;
                case "Microsoft.EntityFrameworkCore.Database.Transaction.TransactionDisposed":
                    break;
                case "Microsoft.EntityFrameworkCore.Database.Transaction.AmbientTransactionWarning":
                    break;
                case "Microsoft.EntityFrameworkCore.Database.Transaction.AmbientTransactionEnlisted":
                    break;
                case "Microsoft.EntityFrameworkCore.Database.Transaction.ExplicitTransactionEnlisted":
                    break;

                case "Microsoft.EntityFrameworkCore.Query.QueryPossibleUnintendedUseOfEqualsWarning":
                    PrometheusCounters.EfCoreQueryWarningsCount.WithLabels("QueryPossibleUnintendedUseOfEqualsWarning").Inc();
                    break;
                case "Microsoft.EntityFrameworkCore.Query.QueryPossibleExceptionWithAggregateOperatorWarning":
                    PrometheusCounters.EfCoreQueryWarningsCount.WithLabels("QueryPossibleExceptionWithAggregateOperatorWarning").Inc();
                    break;
                case "Microsoft.EntityFrameworkCore.Query.ModelValidationKeyDefaultValueWarning":
                    PrometheusCounters.EfCoreQueryWarningsCount.WithLabels("ModelValidationKeyDefaultValueWarning").Inc();
                    break;
                case "Microsoft.EntityFrameworkCore.Query.BoolWithDefaultWarning":
                    PrometheusCounters.EfCoreQueryWarningsCount.WithLabels("BoolWithDefaultWarning").Inc();
                    break;
                case "Microsoft.EntityFrameworkCore.Query.QueryExecutionPlanned":
                    break;

                case "Microsoft.EntityFrameworkCore.Update.BatchReadyForExecution":
                    break;
                case "Microsoft.EntityFrameworkCore.Update.BatchSmallerThanMinBatchSize":
                    break;
            }
        }
    }
}
