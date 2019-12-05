using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Prometheus.Contrib.Core;

namespace Prometheus.EntityFramework.Diagnostics
{
    public class EntityFrameworkListenerHandler : DiagnosticListenerHandler
    {
        private static class PrometheusCounters
        {
            public static readonly Histogram EfCoreRequestsDuration = Metrics.CreateHistogram("efcore_commandss_duration_seconds", "The duration of DB requests processed by an application.");

            public static readonly Counter EfCoreConnectionsTotal = Metrics.CreateCounter("efcore_connections_total", "Total DB connections");
            public static readonly Counter EfCoreConnectionsErrors = Metrics.CreateCounter("efcore_connections_errors", "Total DB connections errors");
            public static readonly Counter EfCoreTransactionsCommitedCount = Metrics.CreateCounter("efcore_transactions_committed_total", "Total committed transactions.");
            public static readonly Counter EfCoreTransactionsRollbackCount = Metrics.CreateCounter("efcore_transactions_rollback_total", "Total rollback transactions.");
            public static readonly Counter EfCoreTransactionsErrorCount = Metrics.CreateCounter("efcore_transactions_error_total", "Total rollback transactions.");
        }

        public EntityFrameworkListenerHandler(string sourceName) : base(sourceName)
        {
        }

        public override void OnCustom(string name, Activity activity, object payload)
        {
            switch (name)
            {
                case "Microsoft.EntityFrameworkCore.Database.Connection.ConnectionOpening":
                    break;
                case "Microsoft.EntityFrameworkCore.Database.Connection.ConnectionOpened":
                    break;
                case "Microsoft.EntityFrameworkCore.Database.Connection.ConnectionClosing":
                    break;
                case "Microsoft.EntityFrameworkCore.Database.Connection.ConnectionClosed":
                    if (payload is ConnectionEndEventData connectionEnd)
                        PrometheusCounters.EfCoreConnectionsTotal.Inc();
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
                            PrometheusCounters.EfCoreRequestsDuration.Observe(commandExecuted.Duration.TotalSeconds);
                    }
                    break;
                case "Microsoft.EntityFrameworkCore.Database.Command.CommandError":
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
                        PrometheusCounters.EfCoreTransactionsErrorCount.Inc();
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
                    break;
                case "Microsoft.EntityFrameworkCore.Query.QueryPossibleExceptionWithAggregateOperatorWarning":
                    break;
                case "Microsoft.EntityFrameworkCore.Query.ModelValidationKeyDefaultValueWarning":
                    break;
                case "Microsoft.EntityFrameworkCore.Query.BoolWithDefaultWarning":
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
