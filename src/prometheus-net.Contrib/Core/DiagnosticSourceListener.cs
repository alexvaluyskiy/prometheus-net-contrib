using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Prometheus.Contrib.Core
{
    internal class DiagnosticSourceListener : IObserver<KeyValuePair<string, object>>
    {
        private readonly DiagnosticListenerHandler handler;

        public DiagnosticSourceListener(DiagnosticListenerHandler handler)
        {
            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(KeyValuePair<string, object> value)
        {
            try
            {
                if (value.Key.EndsWith("Start"))
                    handler.OnStartActivity(Activity.Current, value.Value);
                else if (value.Key.EndsWith("Stop"))
                    handler.OnStopActivity(Activity.Current, value.Value);
                else if (value.Key.EndsWith("Exception"))
                    handler.OnException(Activity.Current, value.Value);
                else
                    handler.OnCustom(value.Key, Activity.Current, value.Value);
            }
            catch
            {
            }
        }
    }
}
