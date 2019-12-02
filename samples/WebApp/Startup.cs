using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit.AspNetCoreIntegration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Microsoft.Data.SqlClient;
using Prometheus;
using MassTransit;
using WebApp.Consumers;

namespace WebApp
{
    public class Startup
    {
        public static ConnectionMultiplexer connection = ConnectionMultiplexer.Connect("");
        public static SqlConnection sqlConnection = new SqlConnection("");

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddMassTransit(
                provider => Bus.Factory.CreateUsingInMemory(factoryConfigurator =>
                {
                    factoryConfigurator.ReceiveEndpoint("test_events", receiveEndpointConfigurator =>
                    {
                        receiveEndpointConfigurator.Consumer<TestConsumer>(provider);
                    });
                }),
                config =>
                {
                    config.AddConsumer<TestConsumer>();
                });

            services.AddPrometheusMonitoring();

            sqlConnection.StatisticsEnabled = true;
            sqlConnection.Open();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Metrics.SuppressDefaultMetrics();
            app.UseMetricServer();

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
