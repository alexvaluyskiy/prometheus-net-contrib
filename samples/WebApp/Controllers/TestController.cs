using System;
using System.Net.Http;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Courier;
using Microsoft.AspNetCore.Mvc;
using WebApp.MassTransit;

namespace WebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IBus bus;

        public TestController(IBus bus)
        {
            this.bus = bus;
        }

        [HttpGet("send")]
        public async Task<IActionResult> TestSend()
        {
            await bus.Publish(new TestCommand
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

            await bus.Publish<SubmitOrder>(new
            {
                OrderId = orderId
            });

            await bus.Publish<OrderAccepted>(new
            {
                OrderId = orderId
            });

            return Ok();
        }

        [HttpGet("courier")]
        public async Task<IActionResult> TestCourier()
        {
            var builder = new RoutingSlipBuilder(NewId.NextGuid());
            builder.AddActivity("DownloadImage", new Uri("queue:test_courier_execute"));
            builder.AddActivity("FilterImage", new Uri("queue:test_courier_execute"));
            var routingSlip = builder.Build();

            await bus.Execute(routingSlip);

            return Ok();
        }
    }
}
