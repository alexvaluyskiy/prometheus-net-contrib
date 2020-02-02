using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Prometheus.Contrib.Core
{
    public class DiagnosticSourceSubscriber : IDisposable, IObserver<DiagnosticListener>
    {
        private readonly Func<string, DiagnosticListenerHandler> handlerFactory;
        private readonly Func<DiagnosticListener, bool> diagnosticSourceFilter;
        private long disposed;
        private IDisposable allSourcesSubscription;
        private readonly List<IDisposable> listenerSubscriptions;

        public DiagnosticSourceSubscriber(
            Func<string, DiagnosticListenerHandler> handlerFactory,
            Func<DiagnosticListener, bool> diagnosticSourceFilter)
        {
            listenerSubscriptions = new List<IDisposable>();
            this.handlerFactory = handlerFactory ?? throw new ArgumentNullException(nameof(handlerFactory));
            this.diagnosticSourceFilter = diagnosticSourceFilter;
        }

        public void Subscribe()
        {
            if (allSourcesSubscription == null)
                allSourcesSubscription = DiagnosticListener.AllListeners.Subscribe(this);
        }

        public void OnNext(DiagnosticListener value)
        {
            if (Interlocked.Read(ref disposed) == 0 &&
                diagnosticSourceFilter(value))
            {
                var handler = handlerFactory(value.Name);
                var listener = new DiagnosticSourceListener(handler);
                var subscription = value.Subscribe(listener);

                lock (listenerSubscriptions)
                    listenerSubscriptions.Add(subscription);
            }
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref disposed, 1, 0) == 1)
                return;

            lock (listenerSubscriptions)
            {
                foreach (var listenerSubscription in listenerSubscriptions)
                    listenerSubscription?.Dispose();

                listenerSubscriptions.Clear();
            }

            allSourcesSubscription?.Dispose();
            allSourcesSubscription = null;
        }
    }
}