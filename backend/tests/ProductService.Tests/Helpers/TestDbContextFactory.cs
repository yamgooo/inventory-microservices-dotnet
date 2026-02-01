using Microsoft.EntityFrameworkCore;
using ProductService.Infrastructure.Data;

namespace ProductService.Tests.Helpers;

public static class TestDbContextFactory
{
    public static ProductContext Create()
    {
        var options = new DbContextOptionsBuilder<ProductContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
            .Options;

        var context = new ProductContext(options);
        
        context.Database.EnsureCreated();

        return context;
    }

    public static void Destroy(ProductContext context)
    {
        context.Database.EnsureDeleted();
        context.Dispose();
    }
}