using Prometheus.Contrib.Core;
using System.Diagnostics;

namespace Prometheus.Contrib.Diagnostic
{
    public class EntityFrameworkListenerHandler : DiagnosticListenerHandler
    {
        private static class PrometheusCounters
        {
            private static readonly Counter DbRequestsCount = Metrics.CreateCounter("db_requests_received_total", "Provides the count of DB requests that have been processed by an application.");
            private static readonly Histogram DbRequestsDuration = Metrics.CreateHistogram("db_request_duration_seconds", "The duration of DB requests processed by an application.");
        }

        public EntityFrameworkListenerHandler(string sourceName) : base(sourceName)
        {
        }

        public override void OnCustom(string name, Activity activity, object payload)
        {
            base.OnCustom(name, activity, payload);
        }
    }
}
