using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;
using ProductService.Infrastructure.Data;
using Shared.Infrastructure.Repositories;

namespace ProductService.Infrastructure.Repositories;

public class ProductRepository(ProductContext context) : BaseRepository<Product>(context), IProductRepository
{
    public Task<IEnumerable<Product>> GetByCategory(string category, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    public async Task<Product?> GetByNameAsync(
        string name, 
        CancellationToken cancellationToken = default)
    {
        return await TableAsNoTracking 
            .FirstOrDefaultAsync(
                p => p.Name == name && !p.IsDeleted, 
                cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty) 
            return false;

        return await TableAsNoTracking
            .AnyAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);
    }
}