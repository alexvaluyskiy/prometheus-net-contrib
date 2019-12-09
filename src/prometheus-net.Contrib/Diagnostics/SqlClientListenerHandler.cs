using System;
using Prometheus.Contrib.Core;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Prometheus.Contrib.Diagnostics
{
    public class SqlClientListenerHandler : DiagnosticListenerHandler
    {
        private static class PrometheusCounters
        {
            public static readonly Histogram SqlCommandsDuration = Metrics.CreateHistogram(
                "sqlclient_command_duration_seconds",
                "The duration of DB requests processed by an application.",
                new HistogramConfiguration { Buckets = Histogram.ExponentialBuckets(0.001, 2, 16) });
            public static readonly Counter SqlCommandsErrors = Metrics.CreateCounter(
                "sqlclient_command_errors_total", 
                "Total DB requests errors",
                new CounterConfiguration { LabelNames = new[] { "type" } });

            public static readonly Counter DbConnectionsOpenTotal = Metrics.CreateCounter("sqlclient_connection_opened_total", "Total opened DB connections");
            public static readonly Counter DbConnectionsCloseTotal = Metrics.CreateCounter("sqlclient_connection_closed_total", "Total closed DB connections");
            public static readonly Counter DbConnectionsErrors = Metrics.CreateCounter("sqlclient_connection_errors_total", "Total DB connections errors");

            public static readonly Counter DbTransactionsCommitedCount = Metrics.CreateCounter("sqlclient_transaction_committed_total", "Total committed transactions.");
            public static readonly Counter DbTransactionsRollbackCount = Metrics.CreateCounter("sqlclient_transaction_rollback_total", "Total rollback transactions.");

            public static readonly Gauge SqlBuffersReceived = Metrics.CreateGauge("sqlclient_received_buffers", "Number of buffers received");
            public static readonly Gauge SqlBuffersSent = Metrics.CreateGauge("sqlclient_sent_buffers", "Number of buffers sent");
            public static readonly Gauge SqlBytesReceived = Metrics.CreateGauge("sqlclient_received_bytes", "Number of bytes received");
            public static readonly Gauge SqlBytesSent = Metrics.CreateGauge("sqlclient_sent_bytes", "Number of bytes sent");
            public static readonly Gauge SqlConnectionTime = Metrics.CreateGauge("sqlclient_connection_time", "Number of bytes sent");
            public static readonly Gauge SqlCursorOpens = Metrics.CreateGauge("sqlclient_cursor_open", "Number of bytes sent");
            public static readonly Gauge SqlExecutionTime = Metrics.CreateGauge("sqlclient_execution_time", "Number of bytes sent");
            public static readonly Gauge SqlIduCount = Metrics.CreateGauge("sqlclient_idu_count", "Number of bytes sent");
            public static readonly Gauge SqlIduRows = Metrics.CreateGauge("sqlclient_idu_rows", "Number of bytes sent");
            public static readonly Gauge SqlNetworkServerTime = Metrics.CreateGauge("sqlclient_network_server_time", "Number of bytes sent");
            public static readonly Gauge SqlPreparedExecs = Metrics.CreateGauge("sqlclient_prepared_exec", "Number of bytes sent");
            public static readonly Gauge SqlPrepares = Metrics.CreateGauge("sqlclient_prepares", "Number of bytes sent");
            public static readonly Gauge SqlSelectCount = Metrics.CreateGauge("sqlclient_select_count", "Number of bytes sent");
            public static readonly Gauge SqlSelectRows = Metrics.CreateGauge("sqlclient_select_rows", "Number of bytes sent");
            public static readonly Gauge SqlServerRoundtrips = Metrics.CreateGauge("sqlclient_server_roundtrips", "Number of bytes sent");
            public static readonly Gauge SqlSumResultSets = Metrics.CreateGauge("sqlclient_sum_result_sets", "Number of bytes sent");
            public static readonly Gauge SqlTransactions = Metrics.CreateGauge("sqlclient_transacions", "Number of bytes sent");
            public static readonly Gauge SqlUnpreparedExecs = Metrics.CreateGauge("sqlclient_unprepared_exec", "Number of bytes sent");
        }

        private readonly PropertyFetcher connectionFetcher = new PropertyFetcher("ConnectionId");
        private readonly PropertyFetcher statisticsFetcher = new PropertyFetcher("Statistics");

        private ConcurrentDictionary<string, Dictionary<string, long>> Statistics = new ConcurrentDictionary<string, Dictionary<string, long>>();
        private readonly AsyncLocal<long> commandTimestampContext = new AsyncLocal<long>();

        public SqlClientListenerHandler(string sourceName) : base(sourceName)
        {
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
                        long operationTimeStamp = Stopwatch.GetTimestamp() - commandTimestampContext.Value;
                        PrometheusCounters.SqlCommandsDuration.Observe(TimeSpan.FromTicks(operationTimeStamp).TotalSeconds);

                        if (statisticsFetcher.Fetch(payload) is IDictionary currentStatistics)
                        {
                            var connection = connectionFetcher.Fetch(payload).ToString();

                            WriteStatisticsMetrics(connection, currentStatistics);
                        }
                    }
                    break;
                case "Microsoft.Data.SqlClient.WriteCommandError":
                    {
                        PrometheusCounters.SqlCommandsErrors.WithLabels("command").Inc();
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
                        PrometheusCounters.DbConnectionsErrors.Inc();
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
                        PrometheusCounters.SqlCommandsErrors.WithLabels("transaction").Inc();
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
                        PrometheusCounters.SqlCommandsErrors.WithLabels("transaction").Inc();
                    }
                    break;
            }
        }

        private void WriteStatisticsMetrics(string connection, IDictionary currentStatistics)
        {
            var aggregatedStats = GetAggregatedStatistics(connection, currentStatistics);

            PrometheusCounters.SqlBuffersReceived.Set(aggregatedStats["BuffersReceived"]);
            PrometheusCounters.SqlBuffersSent.Set(aggregatedStats["BuffersSent"]);
            PrometheusCounters.SqlBytesReceived.Set(aggregatedStats["BytesReceived"]);
            PrometheusCounters.SqlBytesSent.Set(aggregatedStats["BytesSent"]);
            PrometheusCounters.SqlConnectionTime.Set(aggregatedStats["ConnectionTime"]);
            PrometheusCounters.SqlCursorOpens.Set(aggregatedStats["CursorOpens"]);
            PrometheusCounters.SqlExecutionTime.Set(aggregatedStats["ExecutionTime"]);
            PrometheusCounters.SqlIduCount.Set(aggregatedStats["IduCount"]);
            PrometheusCounters.SqlIduRows.Set(aggregatedStats["IduRows"]);
            PrometheusCounters.SqlNetworkServerTime.Set(aggregatedStats["NetworkServerTime"]);
            PrometheusCounters.SqlPreparedExecs.Set(aggregatedStats["PreparedExecs"]);
            PrometheusCounters.SqlPrepares.Set(aggregatedStats["Prepares"]);
            PrometheusCounters.SqlSelectCount.Set(aggregatedStats["SelectCount"]);
            PrometheusCounters.SqlSelectRows.Set(aggregatedStats["SelectRows"]);
            PrometheusCounters.SqlServerRoundtrips.Set(aggregatedStats["ServerRoundtrips"]);
            PrometheusCounters.SqlSumResultSets.Set(aggregatedStats["SumResultSets"]);
            PrometheusCounters.SqlTransactions.Set(aggregatedStats["Transactions"]);
            PrometheusCounters.SqlUnpreparedExecs.Set(aggregatedStats["UnpreparedExecs"]);
        }

        private Dictionary<string, long> GetAggregatedStatistics(string connectionId, IDictionary currentStatistics)
        {
            var genericDictionary = new Dictionary<string, long>();
            foreach (var key in currentStatistics.Keys)
                genericDictionary.Add(key.ToString(), (long)currentStatistics[key]);

            Statistics[connectionId] = genericDictionary;

            return Statistics.SelectMany(x => x.Value)
                .GroupBy(x => x.Key, v => v.Value, (k, g) => new { Key = k, Val = g.Sum(x => x) })
                .ToDictionary(x => x.Key, v => v.Val);
        }
    }
}
