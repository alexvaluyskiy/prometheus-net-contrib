using System.Diagnostics;
using System.Threading;
using Prometheus.Contrib.Core;

namespace Prometheus.MassTransit.Diagnostics
{
    public class MassTransitListenerHandler : DiagnosticListenerHandler
    {
        private static class PrometheusCounters
        {
            private static string[] DefaultLabelNames = new[] { "message" };
            private static string[] ErrorLabelNames = new[] { "exception" };

            public static readonly Histogram SendMessageCount = Metrics.CreateHistogram(
                "masstransit_total_sent_messages",
                "Total sent messages.",
                new HistogramConfiguration { LabelNames = DefaultLabelNames });

            public static readonly Counter SendMessageErrors = Metrics.CreateCounter(
                "masstransit_total_sent_messages_errors",
                "Total sent messages errors",
                new CounterConfiguration { LabelNames = ErrorLabelNames });

            public static readonly Histogram ConsumeMessageCount = Metrics.CreateHistogram(
                "masstransit_total_consumed_messages",
                "The time to consume a message, in seconds.",
                new HistogramConfiguration { LabelNames = DefaultLabelNames });

            public static readonly Histogram ConsumeCriticalDuration = Metrics.CreateHistogram(
                "masstransit_critical_time_seconds",
                "The time between when message is sent and when it is consumed, in seconds.",
                new HistogramConfiguration { LabelNames = DefaultLabelNames });

            public static readonly Counter ConsumeMessageError = Metrics.CreateCounter(
                "masstransit_total_consumed_messages_errors",
                "The number of message processing failures.",
                new CounterConfiguration { LabelNames = ErrorLabelNames });
        }

        private readonly PropertyFetcher messageTypeFetcher = new PropertyFetcher("MessageType");
        private readonly PropertyFetcher consumerTypeFetcher = new PropertyFetcher("ConsumerType");
        private readonly PropertyFetcher exchangeFetcher = new PropertyFetcher("Exchange");

        private AsyncLocal<string> messageTypeContext = new AsyncLocal<string>();
        private AsyncLocal<string> exchangeContext = new AsyncLocal<string>();

        public MassTransitListenerHandler(string sourceName) : base(sourceName)
        {
        }

        public override void OnStartActivity(Activity activity, object payload)
        {
            switch (activity.OperationName)
            {
                case "Transport.Send":
                    {
                        var messageType = exchangeFetcher.Fetch(payload);
                        exchangeContext.Value = messageType.ToString();
                    }
                    break;
                case "Transport.Receive":
                    break;
                case "Consumer.Consume":
                    {
                        var messageType = messageTypeFetcher.Fetch(payload);
                        messageTypeContext.Value = messageType.ToString();
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
                        PrometheusCounters.SendMessageCount
                            .WithLabels(exchangeContext.Value)
                            .Observe(activity.Duration.TotalSeconds);
                    }
                    break;
                case "Consumer.Consume":
                    {
                        PrometheusCounters.ConsumeMessageCount
                            .WithLabels(messageTypeContext.Value)
                            .Observe(activity.Duration.TotalSeconds);

                        // TODO: calculate critical time
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
