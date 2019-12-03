using MassTransit;
using StackExchange.Redis;
using System;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using WebApp;
using WebApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace WebApp.Consumers
{
    public class TestConsumer : IConsumer<TestCommand>
    {
        private readonly SqlConnection connection;
        private readonly TestContext testContext;

        public TestConsumer(SqlConnection connection, TestContext testContext)
        {
            this.connection = connection;
            this.testContext = testContext;
        }

        public async Task Consume(ConsumeContext<TestCommand> context)
        {
            //var database = Startup.connection.GetDatabase();
            //database.StringGet("test1");

            await testContext.TestEntities.ToListAsync();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT 1";
            await command.ExecuteNonQueryAsync();
        }
    }
}
