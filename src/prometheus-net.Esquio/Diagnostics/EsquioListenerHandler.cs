using System.Diagnostics;
using Esquio;
using Esquio.Diagnostics;
using Prometheus.Contrib.Core;

namespace Prometheus.Esquio.Diagnostics
{
    public class EsquioListenerHandler : DiagnosticListenerHandler
    {
        private static class PrometheusCounters
        {
            public static readonly Counter FeatureEvaluation = Metrics.CreateCounter(
                "esquio_feature_evaluation_total",
                "Feature evaluation count",
                new CounterConfiguration { LabelNames = new[] { "product", "feature", "enabled" } });

            public static readonly Counter FeatureEvaluationThrows = Metrics.CreateCounter(
                "esquio_feature_evaluation_throws_total",
                "Feature evaluation throws count",
                new CounterConfiguration { LabelNames = new[] { "product", "feature" } });

            public static readonly Counter FeatureEvaluationNotFound = Metrics.CreateCounter(
                "esquio_feature_evaluation_notfound_total",
                "Feature evaluation not found count",
                new CounterConfiguration { LabelNames = new[] { "product", "feature" } });

            public static readonly Counter ToggleEvaluation = Metrics.CreateCounter(
                "esquio_toggle_evaluation_total",
                "Toggle evaluation count",
                new CounterConfiguration { LabelNames = new[] { "product", "feature", "type" } });
        }

        public EsquioListenerHandler(string sourceName) : base(sourceName)
        {
        }

        public override void OnCustom(string name, Activity activity, object payload)
        {
            switch (activity.OperationName)
            {
                case EsquioConstants.ESQUIO_ENDFEATURE_ACTIVITY_NAME when payload is FeatureEvaluatedEventData featureEvaluated:
                    PrometheusCounters.FeatureEvaluation
                        .WithLabels(featureEvaluated.Product, featureEvaluated.Feature, featureEvaluated.Enabled.ToString())
                        .Inc();
                    break;
                case EsquioConstants.ESQUIO_THROWFEATURE_ACTIVITY_NAME when payload is FeatureThrowEventData featureThrow:
                    PrometheusCounters.FeatureEvaluationThrows
                        .WithLabels(featureThrow.Product, featureThrow.Feature)
                        .Inc();
                    break;
                case EsquioConstants.ESQUIO_NOTFOUNDFEATURE_ACTIVITY_NAME when payload is FeatureNotFoundEventData featureNotFound:
                    PrometheusCounters.FeatureEvaluationNotFound
                        .WithLabels(featureNotFound.Product, featureNotFound.Feature)
                        .Inc();
                    break;
                case EsquioConstants.ESQUIO_ENDTOGGLE_ACTIVITY_NAME when payload is ToggleEvaluatedEventData toggleEvaluated:
                    PrometheusCounters.ToggleEvaluation
                        .WithLabels(toggleEvaluated.Product, toggleEvaluated.Feature, toggleEvaluated.ToggleType)
                        .Inc();
                    break;
            }
        }
    }
}
