using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;
using ProductService.Infrastructure.Data;
using Shared.Infrastructure.Repositories;

namespace ProductService.Infrastructure.Repositories;

public class ProductRepository(ProductContext context) : BaseRepository<Product>(context), IProductRepository
{
    public async Task<Product?> GetByNameAsync(
        string name, 
        CancellationToken cancellationToken = default)
    {
        return await TableAsNoTracking 
            .FirstOrDefaultAsync(
                p => p.Name == name && !p.IsDeleted, 
                cancellationToken);
    }
}