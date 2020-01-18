using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit.AspNetCoreIntegration;
using Microsoft.Data.SqlClient;
using Prometheus;
using MassTransit;
using System;
using GreenPipes;
using MassTransit.Saga;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using WebApp.Data;
using WebApp.MassTransit;

namespace WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<TestContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddSingleton<ISagaRepository<OrderState>>(provider => new InMemorySagaRepository<OrderState>());

            services.AddMassTransit(
                provider => Bus.Factory.CreateUsingRabbitMq(factoryConfigurator =>
                {
                    factoryConfigurator.Host(new Uri("amqp://admin:admin@localhost:5672/"));

                    factoryConfigurator.ReceiveEndpoint("test_events", receiveEndpointConfigurator =>
                    {
                        receiveEndpointConfigurator.Consumer<TestConsumer>(provider);
                    });

                    factoryConfigurator.ReceiveEndpoint("test_saga", receiveEndpointConfigurator =>
                    {
                        receiveEndpointConfigurator.StateMachineSaga<OrderState>(provider);
                    });

                    factoryConfigurator.ReceiveEndpoint("test_courier_execute", receiveEndpointConfigurator =>
                    {
                        var compnsateUri = new Uri("queue:test_courier_compensate");
                        receiveEndpointConfigurator.ExecuteActivityHost<DownloadImageActivity, DownloadImageArguments>(compnsateUri, provider);
                        receiveEndpointConfigurator.ExecuteActivityHost<FilterImageActivity, FilterImageArguments>(compnsateUri, provider);
                    });

                    factoryConfigurator.ReceiveEndpoint("test_courier_compensate", receiveEndpointConfigurator =>
                    {
                        receiveEndpointConfigurator.CompensateActivityHost<DownloadImageActivity, DownloadImageLog>(provider);
                    });
                }),
                config =>
                {
                    config.AddConsumer<TestConsumer>();
                    config.AddSagaStateMachine<OrderStateMachine, OrderState>();

                    config.AddActivity<DownloadImageActivity, DownloadImageArguments, DownloadImageLog>();
                    config.AddActivity<FilterImageActivity, FilterImageArguments, FilterImageLog>();
                }, options =>  options.FailureStatus = HealthStatus.Unhealthy);

            services.AddPrometheusAspNetCoreMetrics();
            services.AddPrometheusMassTransitMetrics();

            services.AddSingleton(provider =>
            {
                var sqlConnection = new SqlConnection(Configuration.GetConnectionString("DefaultConnection"));
                sqlConnection.StatisticsEnabled = true;
                sqlConnection.Open();
                return sqlConnection;
            });
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
                endpoints.MapMetrics();
            });
        }
    }
}
