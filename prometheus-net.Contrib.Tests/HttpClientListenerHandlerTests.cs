using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Prometheus.Contrib.Diagnostics;
using Stubbery;
using Xunit;

namespace prometheus_net.Contrib.Tests
{
    public class HttpClientListenerHandlerTests
    {
        [Fact]
        public async Task When_HttpClient_request_is_cancelled_should_increase_metric()
        {
            var apiStub = new ApiStub();
            apiStub.Get("/", (request, args) =>
            {
                Task.Delay(TimeSpan.FromMinutes(1)).GetAwaiter().GetResult();
                return string.Empty;
            });
            apiStub.Start();

            var services = new ServiceCollection();
            services.AddPrometheusHttpClientMetrics();
            services.AddHttpClient("test").ConfigureHttpClient(options =>
            {
                options.BaseAddress = new Uri(apiStub.Address);
            });

            var provider = services.BuildServiceProvider();
            var client = provider.GetService<IHttpClientFactory>().CreateClient("test");

            var cancellationTokenSource = new CancellationTokenSource(1);

            Assert.ThrowsAsync<TaskCanceledException>(async () =>
                await client.GetAsync("/", cancellationTokenSource.Token));

            Assert.Equal(1,
                HttpClientListenerHandler.PrometheusCounters.HttpClientRequestsDuration
                    .WithLabels("0", client.BaseAddress.Host).Count);
        }
    }
}