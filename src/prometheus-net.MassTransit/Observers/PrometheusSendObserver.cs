using System;
using System.Threading.Tasks;
using MassTransit;

namespace Prometheus.MassTransit.Observers
{
    public class PrometheusSendObserver : ISendObserver
    {
        private static readonly Counter SentMessagesCount = Metrics.CreateCounter(
            "masstransit_messages_sent_total",
            "Total published messages.",
            new CounterConfiguration { LabelNames = new[] { "operation", "message" } });

        private static readonly Counter SentMessagesErrors = Metrics.CreateCounter(
            "masstransit_messages_sent_errors_total",
            "Total published messages errors",
            new CounterConfiguration { LabelNames = new[] { "operation", "exception" } });

        public Task PreSend<T>(SendContext<T> context)
            where T : class
        {
            return Task.CompletedTask;
        }

        public Task PostSend<T>(SendContext<T> context)
            where T : class
        {
            SentMessagesCount
                .WithLabels("Send", context.Message.GetType().FullName)
                .Inc();
            return Task.CompletedTask;
        }

        public Task SendFault<T>(SendContext<T> context, Exception exception)
            where T : class
        {
            SentMessagesErrors
                .WithLabels("Send", exception.GetType().Name)
                .Inc();
            return Task.CompletedTask;
        }
    }
}
