using System.Threading.Tasks;
using IdentityServer4.Events;
using IdentityServer4.Services;

namespace Prometheus.IdentityServer.Sinks
{
    public class PrometheusEventsSink : IEventSink
    {
        private static readonly Counter ClientAuthenticationSuccessCount = Metrics.CreateCounter(
            "idsrv_total_client_authentication_success",
            "Gets raised for successful client authentication at the token endpoint.",
            new CounterConfiguration { LabelNames = new[] { "client" } });

        private static readonly Counter ClientAuthenticationFailureCount = Metrics.CreateCounter(
            "idsrv_total_client_authentication_failure",
            "Gets raised for failed client authentication at the token endpoint.",
            new CounterConfiguration { LabelNames = new[] { "client" } });

        private static readonly Counter TokenIssuedSuccessCount = Metrics.CreateCounter(
            "idsrv_total_token_issued_success",
            "Gets raised for successful attempts to request access tokens.",
            new CounterConfiguration { LabelNames = new[] { "flow" } });

        private static readonly Counter TokenIssuedFailureCount = Metrics.CreateCounter(
            "idsrv_total_token_issued_failure",
            "Gets raised for failed attempts to request access tokens.",
            new CounterConfiguration { LabelNames = new[] { "flow", "error" } });

        private static readonly Counter UserLoginSuccessCount = Metrics.CreateCounter(
            "idsrv_total_user_login_success",
            "Gets raised by the UI for successful user logins.");

        private static readonly Counter UserLoginFailureCount = Metrics.CreateCounter(
            "idsrv_total_user_login_failure",
            "Gets raised by the UI for failed user logins.");

        private static readonly Counter UnhandledExceptionCount = Metrics.CreateCounter(
            "idsrv_total_unhandled_exceptions",
            "Gets raised for unhandled exceptions.");

        public Task PersistAsync(Event evt)
        {
            switch (evt)
            {
                case ClientAuthenticationSuccessEvent clientAuthSuccess:
                    ClientAuthenticationSuccessCount.WithLabels(clientAuthSuccess.ClientId).Inc();
                    break;
                case ClientAuthenticationFailureEvent clientAuthFailure:
                    ClientAuthenticationFailureCount.WithLabels(clientAuthFailure.ClientId).Inc();
                    break;
                case TokenIssuedSuccessEvent tokenIssuedSuccess:
                    TokenIssuedSuccessCount.WithLabels(tokenIssuedSuccess.GrantType).Inc();
                    break;
                case TokenIssuedFailureEvent tokenIssuedFailure:
                    TokenIssuedFailureCount.WithLabels(tokenIssuedFailure.GrantType, tokenIssuedFailure.Error).Inc();
                    break;
                case UserLoginSuccessEvent _:
                    UserLoginSuccessCount.Inc();
                    break;
                case UserLoginFailureEvent _:
                    UserLoginFailureCount.Inc();
                    break;
                case UnhandledExceptionEvent _:
                    UnhandledExceptionCount.Inc();
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
