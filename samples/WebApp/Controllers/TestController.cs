using System;
using System.Net.Http;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using WebApp.MassTransit;

namespace WebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IPublishEndpoint endpoint;

        public TestController(IPublishEndpoint endpoint)
        {
            this.endpoint = endpoint;
        }

        [HttpGet("send")]
        public async Task<IActionResult> TestSend()
        {
            await endpoint.Publish(new TestCommand
            {
                Id = Guid.NewGuid()
            });

            using (var client = new HttpClient())
                await client.GetAsync("https://www.google.com");

            return Ok();
        }

        [HttpGet("saga")]
        public async Task<IActionResult> TestSaga()
        {
            var orderId = Guid.NewGuid();

            await endpoint.Publish<SubmitOrder>(new
            {
                OrderId = orderId
            });

            await endpoint.Publish<OrderAccepted>(new
            {
                OrderId = orderId
            });

            return Ok();
        }
    }
}
