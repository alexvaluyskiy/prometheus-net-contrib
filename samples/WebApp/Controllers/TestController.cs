using System;
using System.Net.Http;
using System.Threading.Tasks;
using EasyCaching.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebApp.Data;

namespace WebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly SqlConnection sqlConnection;
        private readonly TestContext testContext;
        private readonly IEasyCachingProvider cachingProvider;

        public TestController(SqlConnection sqlConnection, TestContext testContext, IEasyCachingProviderFactory cachingFactory)
        {
            this.sqlConnection = sqlConnection;
            this.testContext = testContext;
            this.cachingProvider = cachingFactory.GetCachingProvider("default");
        }

        [HttpGet("http-in")]
        public async Task<IActionResult> HttpIn() => Ok("It's OK");

        [HttpGet("http-out")]
        public async Task<IActionResult> HttpOut()
        {
            using var client = new HttpClient();
            await client.GetAsync("https://www.google.com");
            return Ok("It's OK");
        }

        [HttpGet("sql-query")]
        public async Task<IActionResult> SqlQuery()
        {
            var command = this.sqlConnection.CreateCommand();
            command.CommandText = "SELECT 1";
            await command.ExecuteNonQueryAsync();

            return Ok("It's OK");
        }

        [HttpGet("efcore-query")]
        public async Task<IActionResult> EfCore()
        {
            await this.testContext.Database.CanConnectAsync();

            return Ok("It's OK");
        }

        [HttpGet("easy-caching")]
        public async Task<IActionResult> EasyCaching()
        {
            const string key = "test-sample";

            var cachedData = await this.cachingProvider.GetAsync<TestCommand>(key);
            if (!cachedData.HasValue)
            {
                var data = new TestCommand { Id = Guid.NewGuid() };
                await this.cachingProvider.SetAsync(key, data, TimeSpan.FromSeconds(5));
            }

            var isExists = await this.cachingProvider.ExistsAsync(key);
            if (isExists)
            {
                await this.cachingProvider.RemoveAsync(key);
            }

            return Ok("It's OK");
        }
    }

    public class TestCommand
    {
        public Guid Id { get; set; }
    }
}
