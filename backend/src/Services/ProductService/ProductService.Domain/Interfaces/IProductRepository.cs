using ProductService.Domain.Entities;
using Shared.Infrastructure.Repositories;

namespace ProductService.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}