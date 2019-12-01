using System.Diagnostics;

namespace Prometheus.Contrib.Core
{
    public abstract class DiagnosticListenerHandler
    {
        public DiagnosticListenerHandler(string sourceName)
        {
            SourceName = sourceName;
        }

        public string SourceName { get; }

        public virtual void OnStartActivity(Activity activity, object payload)
        {
        }

        public virtual void OnStopActivity(Activity activity, object payload)
        {
        }

        public virtual void OnException(Activity activity, object payload)
        {
        }

        public virtual void OnCustom(string name, Activity activity, object payload)
        {
        }
    }
}
