using System.Threading.Tasks;
using IdentityServer4.Events;
using IdentityServer4.Services;

namespace Prometheus.IdentityServer.Sinks
{
    public class PrometheusEventsSink : IEventSink
    {
        private static readonly Counter ApiAuthenticationSuccessCount = Metrics.CreateCounter(
            "idsrv_api_authentication_failure_total",
            "Gets raised for successful API authentication at the introspection endpoint.",
            new CounterConfiguration { LabelNames = new[] { "client" } });

        private static readonly Counter ApiAuthenticationFailureCount = Metrics.CreateCounter(
            "idsrv_api_authentication_failure_total",
            "Gets raised for failed API authentication at the introspection endpoint.",
            new CounterConfiguration { LabelNames = new[] { "client" } });

        private static readonly Counter ClientAuthenticationSuccessCount = Metrics.CreateCounter(
            "idsrv_client_authentication_success_total",
            "Gets raised for successful client authentication at the token endpoint.",
            new CounterConfiguration { LabelNames = new[] { "client" } });

        private static readonly Counter ClientAuthenticationFailureCount = Metrics.CreateCounter(
            "idsrv_client_authentication_failure_total",
            "Gets raised for failed client authentication at the token endpoint.",
            new CounterConfiguration { LabelNames = new[] { "client" } });

        private static readonly Counter TokenIssuedSuccessCount = Metrics.CreateCounter(
            "idsrv_token_issued_success_total",
            "Gets raised for successful attempts to request access tokens.",
            new CounterConfiguration { LabelNames = new[] { "flow" } });

        private static readonly Counter TokenIssuedFailureCount = Metrics.CreateCounter(
            "idsrv_token_issued_failure_total",
            "Gets raised for failed attempts to request access tokens.",
            new CounterConfiguration { LabelNames = new[] { "flow", "error" } });

        private static readonly Counter TokenIntrospectionSuccessCount = Metrics.CreateCounter(
            "idsrv_token_introspection_success_total",
            "Gets raised for successful attempts to request identity tokens, access tokens, refresh tokens and authorization codes.");

        private static readonly Counter TokenIntrospectionFailureCount = Metrics.CreateCounter(
            "idsrv_token_introspection_failure_total",
            "Gets raised for failed attempts to request identity tokens, access tokens, refresh tokens and authorization codes.");

        private static readonly Counter TokenRevokedSuccessCount = Metrics.CreateCounter(
            "idsrv_token_revoked_success_total",
            "Gets raised for successful token revocation requests.");

        private static readonly Counter UserLoginSuccessCount = Metrics.CreateCounter(
            "idsrv_user_login_success_total",
            "Gets raised by the UI for successful user logins.");

        private static readonly Counter UserLoginFailureCount = Metrics.CreateCounter(
            "idsrv_user_login_failure_total",
            "Gets raised by the UI for failed user logins.");

        private static readonly Counter UserLogoutSuccessCount = Metrics.CreateCounter(
            "idsrv_user_logout_success_total",
            "Gets raised for successful logout requests.");

        private static readonly Counter ConsentGrantedCount = Metrics.CreateCounter(
            "idsrv_consent_granted_total",
            "Gets raised in the consent UI.");

        private static readonly Counter ConsentDeniedCount = Metrics.CreateCounter(
            "idsrv_consent_denied_total",
            "Gets raised in the consent UI.");

        private static readonly Counter UnhandledExceptionCount = Metrics.CreateCounter(
            "idsrv_unhandled_exceptions_total",
            "Gets raised for unhandled exceptions.");

        private static readonly Counter DeviceAuthorizationSuccessCount = Metrics.CreateCounter(
            "idsrv_device_authorization_success_total",
            "Gets raised for successful device authorization requests.");

        private static readonly Counter DeviceAuthorizationFailureCount = Metrics.CreateCounter(
            "idsrv_device_authorization_success_total",
            "Gets raised for failed device authorization requests.");

        public Task PersistAsync(Event evt)
        {
            switch (evt)
            {
                case ApiAuthenticationFailureEvent _:
                    ApiAuthenticationSuccessCount.Inc();
                    break;
                case ApiAuthenticationSuccessEvent _:
                    ApiAuthenticationFailureCount.Inc();
                    break;
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
                case TokenIntrospectionSuccessEvent _:
                    TokenIntrospectionSuccessCount.Inc();
                    break;
                case TokenIntrospectionFailureEvent _:
                    TokenIntrospectionFailureCount.Inc();
                    break;
                case TokenRevokedSuccessEvent _:
                    TokenRevokedSuccessCount.Inc();
                    break;
                case UserLoginSuccessEvent _:
                    UserLoginSuccessCount.Inc();
                    break;
                case UserLoginFailureEvent _:
                    UserLoginFailureCount.Inc();
                    break;
                case UserLogoutSuccessEvent _:
                    UserLogoutSuccessCount.Inc();
                    break;
                case ConsentGrantedEvent _:
                    ConsentGrantedCount.Inc();
                    break;
                case ConsentDeniedEvent _:
                    ConsentDeniedCount.Inc();
                    break;
                case UnhandledExceptionEvent _:
                    UnhandledExceptionCount.Inc();
                    break;
                case DeviceAuthorizationSuccessEvent _:
                    DeviceAuthorizationSuccessCount.Inc();
                    break;
                case DeviceAuthorizationFailureEvent _:
                    DeviceAuthorizationFailureCount.Inc();
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
