using System;
using Prometheus.Contrib.Core;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Prometheus.Contrib.Options;

namespace Prometheus.Contrib.Diagnostics
{
    public class SqlClientListenerHandler : DiagnosticListenerHandler
    {
        private readonly SqlMetricsOptions options;

        private static class PrometheusCounters
        {
            public static readonly Histogram SqlCommandsDuration = Metrics.CreateHistogram(
                "sqlclient_commands_duration_seconds",
                "The duration of DB requests processed by an application.",
                new HistogramConfiguration { Buckets = Histogram.ExponentialBuckets(0.001, 2, 16) });
            public static readonly Counter SqlCommandsErrors = Metrics.CreateCounter(
                "sqlclient_commands_errors_total",
                "Total DB command errors",
                new CounterConfiguration { LabelNames = new[] { "code" } });

            public static readonly Counter DbConnectionsOpenTotal = Metrics.CreateCounter("sqlclient_connections_opened_total", "Total opened DB connections");
            public static readonly Counter DbConnectionsCloseTotal = Metrics.CreateCounter("sqlclient_connections_closed_total", "Total closed DB connections");
            public static readonly Counter DbConnectionsErrors = Metrics.CreateCounter(
                "sqlclient_connections_errors_total",
                "Total DB connections errors",
                new CounterConfiguration { LabelNames = new[] { "code" } });

            public static readonly Counter DbTransactionsCommitedCount = Metrics.CreateCounter("sqlclient_transactions_committed_total", "Total committed transactions.");
            public static readonly Counter DbTransactionsRollbackCount = Metrics.CreateCounter("sqlclient_transactions_rollback_total", "Total rollback transactions.");
            public static readonly Counter DbTransactionsErrors = Metrics.CreateCounter(
                "sqlclient_transactions_errors_total",
                "Total DB transaction errors",
                new CounterConfiguration { LabelNames = new[] { "code" } });
        }

        private readonly PropertyFetcher<object> commandException = new PropertyFetcher<object>("Exception");
        private readonly PropertyFetcher<int> commandExceptionNumber = new PropertyFetcher<int>("Number");

        private readonly PropertyFetcher<object> connectionException = new PropertyFetcher<object>("Exception");
        private readonly PropertyFetcher<int> connectionExceptionNumber = new PropertyFetcher<int>("Number");

        private readonly PropertyFetcher<object> transactionException = new PropertyFetcher<object>("Exception");
        private readonly PropertyFetcher<int> transactionExceptionNumber = new PropertyFetcher<int>("Number");

        private readonly PropertyFetcher<object> rollbackException = new PropertyFetcher<object>("Exception");
        private readonly PropertyFetcher<int> rollbackExceptionNumber = new PropertyFetcher<int>("Number");

        private readonly AsyncLocal<long> commandTimestampContext = new AsyncLocal<long>();

        public SqlClientListenerHandler(string sourceName, SqlMetricsOptions options) : base(sourceName)
        {
            this.options = options;
        }

        public override void OnCustom(string name, Activity activity, object payload)
        {
            if (name.Contains("WriteCommand"))
                OnWriteCommand(name, payload);
            else if (name.Contains("WriteConnectionOpen"))
                OnWriteConnectionOpen(name, payload);
            else if (name.Contains("WriteConnectionClose"))
                OnWriteConnectionClose(name, payload);
            else if (name.Contains("WriteTransactionCommit"))
                OnWriteTransactionCommit(name, payload);
            else if (name.Contains("WriteTransactionRollback"))
                OnWriteTransactionRollback(name, payload);
        }

        public void OnWriteCommand(string name, object payload)
        {
            switch (name)
            {
                case "Microsoft.Data.SqlClient.WriteCommandBefore":
                    {
                        commandTimestampContext.Value = Stopwatch.GetTimestamp();
                    }
                    break;
                case "Microsoft.Data.SqlClient.WriteCommandAfter":
                    {
                        long ticks = Stopwatch.GetTimestamp() - commandTimestampContext.Value;
                        var timeElapsed = TimeSpan.FromMilliseconds(((double)ticks / Stopwatch.Frequency) * 1000);
                        PrometheusCounters.SqlCommandsDuration.Observe(timeElapsed.TotalSeconds);
                    }
                    break;
                case "Microsoft.Data.SqlClient.WriteCommandError":
                    {
                        if (commandException.TryFetch(payload, out var sqlException))
                        {
                            if (commandExceptionNumber.TryFetch(sqlException, out var errorCode))
                            {
                                PrometheusCounters.SqlCommandsErrors.WithLabels(errorCode.ToString()).Inc();
                            }
                        }
                    }
                    break;
            }
        }

        public void OnWriteConnectionOpen(string name, object payload)
        {
            switch (name)
            {
                case "Microsoft.Data.SqlClient.WriteConnectionOpenBefore":
                    {
                    }
                    break;
                case "Microsoft.Data.SqlClient.WriteConnectionOpenAfter":
                    {
                        PrometheusCounters.DbConnectionsOpenTotal.Inc();
                    }
                    break;
                case "Microsoft.Data.SqlClient.WriteConnectionOpenError":
                    {
                        if (connectionException.TryFetch(payload, out var sqlException))
                        {
                            if (connectionExceptionNumber.TryFetch(sqlException, out var errorCode))
                            {
                                PrometheusCounters.DbConnectionsErrors.WithLabels(errorCode.ToString()).Inc();
                            }
                        }
                    }
                    break;
            }
        }

        public void OnWriteConnectionClose(string name, object payload)
        {
            switch (name)
            {
                case "Microsoft.Data.SqlClient.WriteConnectionCloseBefore":
                    {
                    }
                    break;
                case "Microsoft.Data.SqlClient.WriteConnectionCloseAfter":
                    {
                        PrometheusCounters.DbConnectionsCloseTotal.Inc();
                    }
                    break;
                case "Microsoft.Data.SqlClient.WriteConnectionCloseError":
                    {
                        PrometheusCounters.DbConnectionsErrors.Inc();
                    }
                    break;
            }
        }

        public void OnWriteTransactionCommit(string name, object payload)
        {
            switch (name)
            {
                case "Microsoft.Data.SqlClient.WriteTransactionCommitBefore":
                    {
                    }
                    break;
                case "Microsoft.Data.SqlClient.WriteTransactionCommitAfter":
                    {
                        PrometheusCounters.DbTransactionsCommitedCount.Inc();
                    }
                    break;
                case "Microsoft.Data.SqlClient.WriteTransactionCommitError":
                    {
                        if (transactionException.TryFetch(payload, out var sqlException))
                        {
                            if (transactionExceptionNumber.TryFetch(sqlException, out var errorCode))
                            {
                                PrometheusCounters.DbTransactionsErrors.WithLabels(errorCode.ToString()).Inc();
                            }
                        }
                    }
                    break;
            }
        }

        public void OnWriteTransactionRollback(string name, object payload)
        {
            switch (name)
            {
                case "Microsoft.Data.SqlClient.WriteTransactionRollbackBefore":
                    {
                    }
                    break;
                case "Microsoft.Data.SqlClient.WriteTransactionRollbackAfter":
                    {
                        PrometheusCounters.DbTransactionsRollbackCount.Inc();
                    }
                    break;
                case "Microsoft.Data.SqlClient.WriteTransactionRollbackError":
                    {
                        if (rollbackException.TryFetch(payload, out var sqlException))
                        {
                            if (rollbackExceptionNumber.TryFetch(sqlException, out var errorCode))
                            {
                                PrometheusCounters.DbTransactionsErrors.WithLabels(errorCode.ToString()).Inc();
                            }
                        }
                    }
                    break;
            }
        }
    }
}
