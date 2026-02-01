using TransactionService.Application.Dto;

namespace TransactionService.Application.Services;

public interface IProductServiceClient
{
    Task<ProductDto?> GetProductAsync(Guid productId, CancellationToken cancellationToken = default);

    Task<bool> UpdateStockAsync(Guid productId, int quantityChange, CancellationToken cancellationToken = default);

    Task<bool> ProductExistsAsync(Guid productId, CancellationToken cancellationToken = default);
}