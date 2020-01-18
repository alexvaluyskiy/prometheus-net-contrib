using System;
using System.Threading.Tasks;
using MassTransit;

namespace Prometheus.MassTransit.Observers
{
    public class PrometheusConsumeObserver : IReceiveObserver
    {
        private static readonly Histogram ConsumeTimer = Metrics.CreateHistogram(
            "masstransit_messages_consumed_total",
            "The time to consume a message, in seconds.",
            new HistogramConfiguration { LabelNames = new[] { "message" } });

        private static readonly Histogram CriticalTimer = Metrics.CreateHistogram(
            "masstransit_critical_time_seconds",
            "The time between when message is sent and when it is consumed, in seconds.",
            new HistogramConfiguration { LabelNames = new[] { "message" } });

        private static readonly Counter ErrorCounter = Metrics.CreateCounter(
            "masstransit_messages_consumed_errors_total",
            "The number of message processing failures.",
            new CounterConfiguration { LabelNames = new[] { "exception" } });

        public Task PreReceive(ReceiveContext context) => Task.CompletedTask;
        public Task PostReceive(ReceiveContext context) => Task.CompletedTask;
        public Task ReceiveFault(ReceiveContext context, Exception exception) => Task.CompletedTask;

        public Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType) where T : class
        {
            var messageType = typeof(T).ToString();

            ConsumeTimer.WithLabels(messageType).Observe(duration.TotalSeconds);

            if (context.SentTime != null)
            {
                CriticalTimer.WithLabels(messageType)
                    .Observe((DateTime.UtcNow - context.SentTime.Value).TotalSeconds);
            }

            return Task.CompletedTask;
        }

        public Task ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception) where T : class
        {
            var messageType = typeof(T).ToString();

            ErrorCounter.WithLabels(messageType).Inc();

            return Task.CompletedTask;
        }
    }
}
