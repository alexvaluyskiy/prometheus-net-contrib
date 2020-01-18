using System.Threading.Tasks;
using MassTransit;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;

namespace WebApp.MassTransit
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
            await testContext.TestEntities.ToListAsync();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT 1";
            await command.ExecuteNonQueryAsync();
        }
    }
}
