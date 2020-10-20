using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebApp.Data;

namespace WebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MonitoringController : ControllerBase
    {
        private readonly SqlConnection sqlConnection;
        private readonly TestContext testContext;

        public MonitoringController(SqlConnection sqlConnection, TestContext testContext)
        {
            this.sqlConnection = sqlConnection;
            this.testContext = testContext;
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
    }
}
