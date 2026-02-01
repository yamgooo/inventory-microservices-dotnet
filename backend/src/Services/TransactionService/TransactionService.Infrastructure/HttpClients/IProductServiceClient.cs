using ProductService.Application.Dto;

namespace TransactionService.Infrastructure.HttpClients;

public interface IProductServiceClient
{
    Task<ProductDto?> GetProductAsync(Guid productId, CancellationToken cancellationToken = default);

    Task<bool> UpdateStockAsync(Guid productId, int quantityChange, CancellationToken cancellationToken = default);

    Task<bool> ProductExistsAsync(Guid productId, CancellationToken cancellationToken = default);

    public Task<Dictionary<Guid, ProductDto>> GetProductsByIdsAsync(IEnumerable<Guid> productIds, CancellationToken cancellationToken = default);
}