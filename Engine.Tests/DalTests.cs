using Engine.Dal;
using Engine.Field;
using Engine.Field.InternalEntities;
using Engine.Field.Map;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Engine.Tests;

public class DalTests
{
    private static readonly Mock<IDbContextFactory<BeeDbContext>> MockDbFactory = new();
    
    public DalTests()
    {
        MockDbFactory.Setup(f => f.CreateDbContext())
            .Returns(() => new BeeDbContext(new DbContextOptionsBuilder<BeeDbContext>().UseSqlite("Data Source=test.db").Options));
    }

    [Test]
    public async ValueTask Test()
    {
        await using var context = MockDbFactory.Object.CreateDbContext();
        context.Dimensions.Add(new Dimension()
        {
            Id = 2,
        });
        await context.SaveChangesAsync();
    }
}