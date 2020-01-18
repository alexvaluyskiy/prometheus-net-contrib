using System.Diagnostics;
using System.Threading;
using Prometheus.Contrib.Core;

namespace Prometheus.MassTransit.Diagnostics
{
    public class MassTransitListenerHandler : DiagnosticListenerHandler
    {
        private static class PrometheusCounters
        {
            public static readonly Histogram SendMessageCount = Metrics.CreateHistogram(
                "masstransit_messages_sent_total",
                "Total sent messages.",
                new HistogramConfiguration { LabelNames = new[] { "exchange" } });

            public static readonly Counter SendMessageErrors = Metrics.CreateCounter(
                "masstransit_messages_sent_errors_total",
                "Total sent messages errors",
                new CounterConfiguration { LabelNames = new[] { "exception" } });

            public static readonly Histogram ConsumeMessageCount = Metrics.CreateHistogram(
                "masstransit_messages_consumed_total",
                "The time to consume a message, in seconds.",
                new HistogramConfiguration { LabelNames = new[] { "consumer", "message" } });

            public static readonly Counter ConsumeMessageError = Metrics.CreateCounter(
                "masstransit_messages_consumed_errors_total",
                "The number of message processing failures.",
                new CounterConfiguration { LabelNames = new[] { "exception" } });
        }

        private readonly PropertyFetcher exchangeFetcher = new PropertyFetcher("Exchange");
        private readonly PropertyFetcher messageTypeFetcher = new PropertyFetcher("MessageType");
        private readonly PropertyFetcher consumerTypeFetcher = new PropertyFetcher("ConsumerType");

        private readonly AsyncLocal<string> exchangeContext = new AsyncLocal<string>();
        private readonly AsyncLocal<(string, string)> messageTypeContext = new AsyncLocal<(string, string)>();

        public MassTransitListenerHandler(string sourceName) : base(sourceName)
        {
        }

        public override void OnStartActivity(Activity activity, object payload)
        {
            switch (activity.OperationName)
            {
                case "Transport.Send":
                    {
                        var exchangeType = exchangeFetcher.Fetch(payload);
                        exchangeContext.Value = exchangeType.ToString();
                    }
                    break;
                case "Transport.Receive":
                    break;
                case "Consumer.Consume":
                    {
                        var messageType = messageTypeFetcher.Fetch(payload);
                        var consumerType = consumerTypeFetcher.Fetch(payload);

                        messageTypeContext.Value = (messageType.ToString(), consumerType.ToString());

                    }
                    break;
            }
        }

        public override void OnStopActivity(Activity activity, object payload)
        {
            switch (activity.OperationName)
            {
                case "Transport.Send":
                    {
                        var exchangeType = exchangeContext.Value;
                        PrometheusCounters.SendMessageCount
                            .WithLabels(exchangeType)
                            .Observe(activity.Duration.TotalSeconds);
                    }
                    break;
                case "Consumer.Consume":
                    {
                        var (messageType, consumerType) = messageTypeContext.Value;

                        PrometheusCounters.ConsumeMessageCount
                            .WithLabels(consumerType, messageType)
                            .Observe(activity.Duration.TotalSeconds);
                    }
                    break;
            }
        }

        public override void OnException(Activity activity, object payload)
        {
            switch (activity.OperationName)
            {
                case "Transport.Send":
                    PrometheusCounters.SendMessageErrors.Inc();
                    break;
                case "Consumer.Consume":
                    PrometheusCounters.ConsumeMessageError.Inc();
                    break;
            }
        }
    }
}
