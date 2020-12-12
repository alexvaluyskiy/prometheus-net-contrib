using Prometheus.Contrib.EventListeners.Counters;

namespace Prometheus.Contrib.EventListeners.Adapters
{
    internal class PrometheusNetSecurityCounterAdapter : BaseAdapter
    {
        public const string EventSourceName = "System.Net.Security";

        internal readonly IncrementCounter TlsHandshakeRate = new IncrementCounter("tls-handshake-rate", "net_security_tls_handshakes_per_second", "TLS handshakes completed");
        internal readonly MeanCounter TotalTlsHandshakes = new MeanCounter("total-tls-handshakes", "net_security_tls_handshakes_total", "Total TLS handshakes completed");
        internal readonly MeanCounter CurrentTlsHandshakes = new MeanCounter("current-tls-handshakes", "net_security_tls_handshakes_current_total", "Current TLS handshakes");
        internal readonly MeanCounter FailedTlsHandshakes = new MeanCounter("failed-tls-handshakes", "net_security_tls_handshakes_failed_total", "Total TLS handshakes failed");
        internal readonly MeanCounter AllTlsSessionsOpen = new MeanCounter("all-tls-sessions-open", "net_security_tls_sessions_total", "All TLS Sessions Active");
        internal readonly MeanCounter Tls10SessionsOpen = new MeanCounter("tls10-sessions-open", "net_security_tls_10_sessions_total", "TLS 1.0 Sessions Active");
        internal readonly MeanCounter Tls11SessionsOpen = new MeanCounter("tls11-sessions-open", "net_security_tls_11_sessions_total", "TLS 1.1 Sessions Active");
        internal readonly MeanCounter Tls12SessionsOpen = new MeanCounter("tls12-sessions-open", "net_security_tls_12_sessions_total", "TLS 1.2 Sessions Active");
        internal readonly MeanCounter Tls13SessionsOpen = new MeanCounter("tls13-sessions-open", "net_security_tls_13_sessions_total", "TLS 1.3 Sessions Active");
        internal readonly MeanCounter AllTlsHandshakeDuration = new MeanCounter("all-tls-handshake-duration", "net_security_handshakes_duration_milliseconds", "TLS Handshake Duration");
        internal readonly MeanCounter Tls10HandshakeDuration = new MeanCounter("tls10-handshake-duration", "net_security_handshakes_tls10_duration_milliseconds", "TLS 1.0 Handshake Duration");
        internal readonly MeanCounter Tls11HandshakeDuration = new MeanCounter("tls11-handshake-duration", "net_security_handshakes_tls11_duration_milliseconds", "TLS 1.1 Handshake Duration");
        internal readonly MeanCounter Tls12HandshakeDuration = new MeanCounter("tls12-handshake-duration", "net_security_handshakes_tls12_duration_milliseconds", "TLS 1.2 Handshake Duration");
        internal readonly MeanCounter Tls13HandshakeDuration = new MeanCounter("tls13-handshake-duration", "net_security_handshakes_tls13_duration_milliseconds", "TLS 1.3 Handshake Duration");
    }
}