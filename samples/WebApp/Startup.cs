using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Data.SqlClient;
using Prometheus;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebApp.Data;

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

            services.AddSingleton(provider =>
            {
                var sqlConnection = new SqlConnection(Configuration.GetConnectionString("DefaultConnection"));
                sqlConnection.StatisticsEnabled = true;
                sqlConnection.Open();
                return sqlConnection;
            });

            services.AddPrometheusAspNetCoreMetrics();
            services.AddPrometheusEasyCachingMetrics();

            AddCachingExtensions(services);
        }

        private void AddCachingExtensions(IServiceCollection services)
        {
            services.AddEasyCaching(options =>
            {
                var connectionString = Configuration.GetConnectionString("RedisConnection");

                if (!string.IsNullOrEmpty(connectionString))
                {
                    options.UseRedis(
                        config =>
                        {
                            config.DBConfig.Configuration = connectionString;
                            config.SerializerName = "json";
                            config.EnableLogging = true;
                        },
                        "default");
                }
                else
                {
                    options.UseInMemory("default");
                }

                options.WithJson(
                    jsonSerializerSettingsConfigure: json => json.ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    "json");
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
