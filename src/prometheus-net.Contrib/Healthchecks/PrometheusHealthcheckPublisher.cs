using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;

namespace Prometheus.Contrib.Healthchecks
{        
    public class PrometheusHealthcheckPublisher : IHealthCheckPublisher
    {
        private static readonly Gauge Healthcheck = Metrics.CreateGauge(
            "health_check",
            "Prometheus healthcheck.",
            new GaugeConfiguration { LabelNames = new[] { "serviceName" } });

        public Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
        {
            foreach (var entry in report.Entries)
            {
                Healthcheck.WithLabels(entry.Key).Set(Convert.ToDouble(entry.Value.Status == HealthStatus.Healthy));
            }

            return Task.CompletedTask;
        }
    }
}
