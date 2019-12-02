using Prometheus.Contrib.Core;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Prometheus.Contrib.Diagnostic
{
    public class SqlClientListenerHandler : DiagnosticListenerHandler
    {
        private static class PrometheusCounters
        {
            public static readonly Histogram SqlCommandsDuration = Metrics.CreateHistogram("sqlclient_commands_duration_seconds", "The duration of DB requests processed by an application.", new HistogramConfiguration { Buckets = Histogram.ExponentialBuckets(0.001, 2, 16) });
            public static readonly Counter SqlCommandsErrors = Metrics.CreateCounter("sqlclient_commands_errors", "Total DB requests errors");

            public static readonly Counter DbConnectionsTotal = Metrics.CreateCounter("sqlclient_onnections_total", "Total DB connections");
            public static readonly Counter DbConnectionsErrors = Metrics.CreateCounter("sqlclient_connections_errors", "Total DB connections errors");
            public static readonly Counter DbTransactionsCommitedCount = Metrics.CreateCounter("sqlclient_transactions_committed_total", "Total committed transactions.");
            public static readonly Counter DbTransactionsRollbackCount = Metrics.CreateCounter("sqlclient_transactions_rollback_total", "Total rollback transactions.");

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
        private readonly PropertyFetcher exceptionFetcher = new PropertyFetcher("Exception");
        private readonly PropertyFetcher statisticsFetcher = new PropertyFetcher("Statistics");

        private Dictionary<string, Dictionary<string, long>> Statistics = new Dictionary<string, Dictionary<string, long>>();
        private readonly AsyncLocal<long> commandTimestampContext = new AsyncLocal<long>();

        public SqlClientListenerHandler(string sourceName) : base(sourceName)
        {
        }

        public override void OnCustom(string name, Activity activity, object payload)
        {
            if (name.Contains("WriteCommand"))
            {
                OnWriteCommand(name, payload);
            }
            else if (name.Contains("WriteConnectionOpen"))
            {
                OnWriteConnectionOpen(name, payload);
            }
            else if (name.Contains("WriteConnectionClose"))
            {
                OnWriteConnectionClose(name, payload);
            }
            else if (name.Contains("WriteTransactionCommit"))
            {
                OnWriteTransactionCommit(name, payload);
            }
            else if (name.Contains("WriteTransactionRollback"))
            {
                OnWriteTransactionRollback(name, payload);
            }
        }

        public void OnWriteCommand(string name, object payload)
        {
            switch (name)
            {
                case "System.Data.SqlClient.WriteCommandBefore":
                case "Microsoft.Data.SqlClient.WriteCommandBefore":
                    {
                        commandTimestampContext.Value = Stopwatch.GetTimestamp();
                    }
                    break;
                case "System.Data.SqlClient.WriteCommandAfter":
                case "Microsoft.Data.SqlClient.WriteCommandAfter":
                    {
                        var commandTimestamp = commandTimestampContext.Value;
                        PrometheusCounters.SqlCommandsDuration.Observe(commandTimestamp);

                        if (statisticsFetcher.Fetch(payload) is IDictionary currentStatistics)
                        {
                            var connection = connectionFetcher.Fetch(payload).ToString();

                            WriteStatisticsMetrics(connection, currentStatistics);
                        }
                    }
                    break;
                case "System.Data.SqlClient.WriteCommandError":
                case "Microsoft.Data.SqlClient.WriteCommandError":
                    {
                        var exception = exceptionFetcher.Fetch(payload);
                        // TODO: add exception message
                        PrometheusCounters.SqlCommandsErrors.Inc();
                    }
                    break;
            }
        }

        public void OnWriteConnectionOpen(string name, object payload)
        {
            switch (name)
            {
                case "System.Data.SqlClient.WriteConnectionOpenBefore":
                case "Microsoft.Data.SqlClient.WriteConnectionOpenBefore":
                    {
                    }
                    break;
                case "System.Data.SqlClient.WriteConnectionOpenAfter":
                case "Microsoft.Data.SqlClient.WriteConnectionOpenAfter":
                    {
                    }
                    break;
                case "System.Data.SqlClient.WriteConnectionOpenError":
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
                case "System.Data.SqlClient.WriteConnectionCloseBefore":
                case "Microsoft.Data.SqlClient.WriteConnectionCloseBefore":
                    {
                    }
                    break;
                case "System.Data.SqlClient.WriteConnectionCloseAfter":
                case "Microsoft.Data.SqlClient.WriteConnectionCloseAfter":
                    {
                        PrometheusCounters.DbConnectionsTotal.Inc();
                    }
                    break;
                case "System.Data.SqlClient.WriteConnectionCloseError":
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
                case "System.Data.SqlClient.WriteTransactionCommitBefore":
                case "Microsoft.Data.SqlClient.WriteTransactionCommitBefore":
                    {
                    }
                    break;
                case "System.Data.SqlClient.WriteTransactionCommitAfter":
                case "Microsoft.Data.SqlClient.WriteTransactionCommitAfter":
                    {
                        PrometheusCounters.DbTransactionsCommitedCount.Inc();
                    }
                    break;
                case "System.Data.SqlClient.WriteTransactionCommitError":
                case "Microsoft.Data.SqlClient.WriteTransactionCommitError":
                    {
                        PrometheusCounters.SqlCommandsErrors.Inc();
                    }
                    break;
            }
        }

        public void OnWriteTransactionRollback(string name, object payload)
        {
            switch (name)
            {
                case "System.Data.SqlClient.WriteTransactionRollbackBefore":
                case "Microsoft.Data.SqlClient.WriteTransactionRollbackBefore":
                    {
                    }
                    break;
                case "System.Data.SqlClient.WriteTransactionRollbackAfter":
                case "Microsoft.Data.SqlClient.WriteTransactionRollbackAfter":
                    {
                        PrometheusCounters.DbTransactionsRollbackCount.Inc();
                    }
                    break;
                case "System.Data.SqlClient.WriteTransactionRollbackError":
                case "Microsoft.Data.SqlClient.WriteTransactionRollbackError":
                    {
                        PrometheusCounters.SqlCommandsErrors.Inc();
                    }
                    break;
            }
        }

        private void WriteStatisticsMetrics(string connection, IDictionary currentStatistics)
        {
            var genericDictionary = new Dictionary<string, long>();
            foreach (var key in currentStatistics.Keys)
            {
                genericDictionary.Add(key.ToString(), (long)currentStatistics[key]);
            }

            Statistics[connection] = genericDictionary;

            PrometheusCounters.SqlBuffersReceived.Set(Statistics.SelectMany(x => x.Value).Where(x => x.Key == "BuffersReceived").Select(x => x.Value).Sum());
            PrometheusCounters.SqlBuffersSent.Set(Statistics.SelectMany(x => x.Value).Where(x => x.Key == "BuffersSent").Select(x => x.Value).Sum());
            PrometheusCounters.SqlBytesReceived.Set(Statistics.SelectMany(x => x.Value).Where(x => x.Key == "BytesReceived").Select(x => x.Value).Sum());
            PrometheusCounters.SqlBytesSent.Set(Statistics.SelectMany(x => x.Value).Where(x => x.Key == "BytesSent").Select(x => x.Value).Sum());
            PrometheusCounters.SqlConnectionTime.Set(Statistics.SelectMany(x => x.Value).Where(x => x.Key == "ConnectionTime").Select(x => x.Value).Sum());
            PrometheusCounters.SqlCursorOpens.Set(Statistics.SelectMany(x => x.Value).Where(x => x.Key == "CursorOpens").Select(x => x.Value).Sum());
            PrometheusCounters.SqlExecutionTime.Set(Statistics.SelectMany(x => x.Value).Where(x => x.Key == "ExecutionTime").Select(x => x.Value).Sum());
            PrometheusCounters.SqlIduCount.Set(Statistics.SelectMany(x => x.Value).Where(x => x.Key == "IduCount").Select(x => x.Value).Sum());
            PrometheusCounters.SqlIduRows.Set(Statistics.SelectMany(x => x.Value).Where(x => x.Key == "IduRows").Select(x => x.Value).Sum());
            PrometheusCounters.SqlNetworkServerTime.Set(Statistics.SelectMany(x => x.Value).Where(x => x.Key == "NetworkServerTime").Select(x => x.Value).Sum());
            PrometheusCounters.SqlPreparedExecs.Set(Statistics.SelectMany(x => x.Value).Where(x => x.Key == "PreparedExecs").Select(x => x.Value).Sum());
            PrometheusCounters.SqlPrepares.Set(Statistics.SelectMany(x => x.Value).Where(x => x.Key == "Prepares").Select(x => x.Value).Sum());
            PrometheusCounters.SqlSelectCount.Set(Statistics.SelectMany(x => x.Value).Where(x => x.Key == "SelectCount").Select(x => x.Value).Sum());
            PrometheusCounters.SqlSelectRows.Set(Statistics.SelectMany(x => x.Value).Where(x => x.Key == "SelectRows").Select(x => x.Value).Sum());
            PrometheusCounters.SqlServerRoundtrips.Set(Statistics.SelectMany(x => x.Value).Where(x => x.Key == "ServerRoundtrips").Select(x => x.Value).Sum());
            PrometheusCounters.SqlSumResultSets.Set(Statistics.SelectMany(x => x.Value).Where(x => x.Key == "SumResultSets").Select(x => x.Value).Sum());
            PrometheusCounters.SqlTransactions.Set(Statistics.SelectMany(x => x.Value).Where(x => x.Key == "Transactions").Select(x => x.Value).Sum());
            PrometheusCounters.SqlUnpreparedExecs.Set(Statistics.SelectMany(x => x.Value).Where(x => x.Key == "UnpreparedExecs").Select(x => x.Value).Sum());
        }
    }
}
