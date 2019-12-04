using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit.AspNetCoreIntegration;
using StackExchange.Redis;
using Microsoft.Data.SqlClient;
using Prometheus;
using MassTransit;
using WebApp.Consumers;
using MassTransit.ActiveMqTransport;
using MassTransit.ActiveMqTransport.Configurators;
using System;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using Microsoft.AspNetCore.Routing;

namespace WebApp
{
    public class Startup
    {
        public static ConnectionMultiplexer connection = ConnectionMultiplexer.Connect("");

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

            services.AddMassTransit(
                provider => Bus.Factory.CreateUsingActiveMq(factoryConfigurator =>
                {
                    var host = factoryConfigurator.Host(new ConfigurationHostSettings(new Uri("activemq://localhost:61616")));
                    factoryConfigurator.ReceiveEndpoint("test_events", receiveEndpointConfigurator =>
                    {
                        receiveEndpointConfigurator.Consumer<TestConsumer>(provider);
                    });
                }),
                config =>
                {
                    config.AddConsumer<TestConsumer>();
                });

            services.AddPrometheusAspNetCoreMetrics();

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
