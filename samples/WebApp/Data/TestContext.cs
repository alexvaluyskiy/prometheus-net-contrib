using Microsoft.EntityFrameworkCore;

namespace WebApp.Data
{
    public class TestContext : DbContext
    {
        public TestContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<TestEntity> TestEntities { get; set; }
    }

    public class TestEntity
    {
        public int Id { get; set; }
    }
}
