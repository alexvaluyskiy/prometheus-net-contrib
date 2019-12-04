using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace Prometheus.Contrib.Endpoints
{
    public class MetricEndpointMiddleware
    {
        public MetricEndpointMiddleware(RequestDelegate next, Settings settings)
        {
            _next = next;

            _registry = settings.Registry ?? Metrics.DefaultRegistry;
        }

        public sealed class Settings
        {
            public CollectorRegistry Registry { get; set; }
        }

        private readonly RequestDelegate _next;

        private readonly CollectorRegistry _registry;

        public async Task Invoke(HttpContext context)
        {
            var response = context.Response;

            try
            {
                response.ContentType = PrometheusConstants.ExporterContentType;
                response.StatusCode = StatusCodes.Status200OK;

                await _registry.CollectAndExportAsTextAsync(response.Body);

                response.Body.Dispose();
            }
            catch (ScrapeFailedException ex)
            {
                // This can only happen before any serialization occurs, in the pre-collect callbacks.
                // So it should still be safe to update the status code and write an error message.
                response.StatusCode = StatusCodes.Status503ServiceUnavailable;

                if (!string.IsNullOrWhiteSpace(ex.Message))
                    using (var writer = new StreamWriter(response.Body))
                        await writer.WriteAsync(ex.Message);
            }
        }
    }
}
