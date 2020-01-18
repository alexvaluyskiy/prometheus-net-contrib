using System;
using System.Threading.Tasks;
using MassTransit;

namespace Prometheus.MassTransit.Observers
{
    public class PrometheusPublishObserver : IPublishObserver
    {
        private static readonly Counter SentMessagesCount = Metrics.CreateCounter(
            "masstransit_messages_sent_total",
            "Total published messages.",
            new CounterConfiguration { LabelNames = new[] { "operation", "message" } });

        private static readonly Counter SentMessagesErrors = Metrics.CreateCounter(
            "masstransit_messages_sent_errors_total",
            "Total published messages errors",
            new CounterConfiguration { LabelNames = new[] { "operation", "exception" } });

        public Task PrePublish<T>(PublishContext<T> context)
            where T : class
        {
            return Task.CompletedTask;
        }

        public Task PostPublish<T>(PublishContext<T> context)
            where T : class
        {
            SentMessagesCount
                .WithLabels("Publish", context.Message.GetType().ToString())
                .Inc();
            return Task.CompletedTask;
        }

        public Task PublishFault<T>(PublishContext<T> context, Exception exception)
            where T : class
        {
            SentMessagesErrors
                .WithLabels("Publish", exception.GetType().Name)
                .Inc();

            return Task.CompletedTask;
        }
    }
}
