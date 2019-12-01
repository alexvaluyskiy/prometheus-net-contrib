using MassTransit;
using StackExchange.Redis;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using WebApp;

namespace WebApp.Consumers
{
    public class TestConsumer : IConsumer<TestCommand>
    {
        public async Task Consume(ConsumeContext<TestCommand> context)
        {
            var database = Startup.connection.GetDatabase();
            database.StringGet("test1");

            var command = Startup.sqlConnection.CreateCommand();
            command.CommandText = "SELECT 1";
            await command.ExecuteNonQueryAsync();
        }
    }
}
