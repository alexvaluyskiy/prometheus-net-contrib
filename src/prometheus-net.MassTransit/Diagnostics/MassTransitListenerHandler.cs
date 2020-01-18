using System.Diagnostics;
using System.Linq;
using MassTransit.Logging;
using Prometheus.Contrib.Core;

namespace Prometheus.MassTransit.Diagnostics
{
    public class MassTransitListenerHandler : DiagnosticListenerHandler
    {
        private static class PrometheusCounters
        {
            public static readonly Histogram SendMessageCount = Metrics.CreateHistogram(
                "masstransit_messages_sent_total",
                "Total sent messages.");

            public static readonly Counter SendMessageErrors = Metrics.CreateCounter(
                "masstransit_messages_sent_errors_total",
                "Total sent messages errors",
                new CounterConfiguration { LabelNames = new[] { "exception" } });

            public static readonly Histogram ReceiveMessageCount = Metrics.CreateHistogram(
                "masstransit_messages_received_total",
                "The time to receive a message, in seconds.",
                new HistogramConfiguration { LabelNames = new[] { "message" } });

            public static readonly Counter ReceiveMessageError = Metrics.CreateCounter(
                "masstransit_messages_received_errors_total",
                "The number of message processing failures.",
                new CounterConfiguration { LabelNames = new[] { "exception" } });

            public static readonly Histogram ConsumeMessageCount = Metrics.CreateHistogram(
                "masstransit_messages_consumed_total",
                "The time to consume a message, in seconds.",
                new HistogramConfiguration { LabelNames = new[] { "consumer" } });

            public static readonly Counter ConsumeMessageError = Metrics.CreateCounter(
                "masstransit_messages_consumed_errors_total",
                "The number of message processing failures.",
                new CounterConfiguration { LabelNames = new[] { "exception" } });
        }

        public MassTransitListenerHandler(string sourceName) : base(sourceName)
        {
        }

        public override void OnStartActivity(Activity activity, object payload)
        {
        }

        public override void OnStopActivity(Activity activity, object payload)
        {
            switch (activity.OperationName)
            {
                case OperationName.Transport.Send:
                    {
                        PrometheusCounters.SendMessageCount.Observe(activity.Duration.TotalSeconds);
                    }
                    break;
                case OperationName.Transport.Receive:
                    {
                        var messageType = activity.Tags
                            .Where(c => c.Key == DiagnosticHeaders.MessageTypes)
                            .Select(c => c.Value)
                            .FirstOrDefault();

                        PrometheusCounters.ReceiveMessageCount
                            .WithLabels(messageType)
                            .Observe(activity.Duration.TotalSeconds);
                    }
                    break;
                case OperationName.Consumer.Consume:
                    {
                        var consumerType = activity.Tags
                            .Where(c => c.Key == DiagnosticHeaders.ConsumerType)
                            .Select(c => c.Value)
                            .FirstOrDefault();

                        PrometheusCounters.ConsumeMessageCount
                            .WithLabels(consumerType)
                            .Observe(activity.Duration.TotalSeconds);
                    }
                    break;
                case OperationName.Consumer.Handle:
                    {
                        PrometheusCounters.ConsumeMessageCount
                            .Observe(activity.Duration.TotalSeconds);
                    }
                    break;
            }
        }

        public override void OnException(Activity activity, object payload)
        {
            switch (activity.OperationName)
            {
                case OperationName.Transport.Send:
                    PrometheusCounters.SendMessageErrors.Inc();
                    break;
                case OperationName.Transport.Receive:
                    PrometheusCounters.ReceiveMessageError.Inc();
                    break;
                case OperationName.Consumer.Consume:
                    PrometheusCounters.ConsumeMessageError.Inc();
                    break;
            }
        }
    }
}
