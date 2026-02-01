using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using ProductService.Application.Dto;
using Shared.Common.Models;

namespace TransactionService.Infrastructure.HttpClients;

public class ProductServiceClient(HttpClient httpClient, ILogger<ProductServiceClient> logger)
    : IProductServiceClient
{
    public async Task<ProductDto?> GetProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Fetching product {ProductId} from ProductService", productId);

            var response = await httpClient.GetAsync($"api/products/{productId}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "Product {ProductId} not found. Status: {StatusCode}", 
                    productId, 
                    response.StatusCode);
                return null;
            }

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<ProductDto>>(cancellationToken);

            if (apiResponse?.Data == null)
            {
                logger.LogWarning("Product {ProductId} returned null data", productId);
                return null;
            }

            logger.LogInformation(
                "Product {ProductId} fetched successfully. Quantity: {Quantity}", 
                productId, 
                apiResponse.Data.Stock);

            return apiResponse.Data;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HTTP error fetching product {ProductId}", productId);
            throw new InvalidOperationException("Unable to communicate with ProductService", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching product {ProductId}", productId);
            throw;
        }
    }

    public async Task<bool> UpdateStockAsync(
        Guid productId, 
        int quantityChange, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation(
                "Updating stock for product {ProductId} by {QuantityChange}", 
                productId, 
                quantityChange);

            var request = new
            {
                Id = productId,
                Quantity = quantityChange
            };

            var response = await httpClient.PutAsJsonAsync(
                $"api/products/{productId}/stock",
                request, 
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                logger.LogError(
                    "Failed to update stock for product {ProductId}. Status: {StatusCode}, Error: {Error}",
                    productId,
                    response.StatusCode,
                    errorContent);
                return false;
            }

            logger.LogInformation("Quantity updated successfully for product {ProductId}", productId);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating stock for product {ProductId}", productId);
            return false;
        }
    }

    public async Task<bool> ProductExistsAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var product = await GetProductAsync(productId, cancellationToken);
        return product != null;
    }
    
    public async Task<Dictionary<Guid, ProductDto>> GetProductsByIdsAsync(
        IEnumerable<Guid> productIds, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var ids = productIds.Distinct().ToList();
            
            if (ids.Count == 0)
            {
                logger.LogDebug("No product IDs provided for batch fetch");
                return new Dictionary<Guid, ProductDto>();
            }

            logger.LogDebug("Fetching {Count} products in batch", ids.Count);

            var response = await httpClient.PostAsJsonAsync(
                "api/products/batch", 
                new { ids }, 
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Failed to fetch products in batch. Status: {StatusCode}", 
                    response.StatusCode);
                return new Dictionary<Guid, ProductDto>();
            }

            var apiResponse = await response.Content
                .ReadFromJsonAsync<ApiResponse<List<ProductDto>>>(cancellationToken);

            if (apiResponse?.Data == null)
            {
                logger.LogWarning("Batch fetch returned null data");
                return new Dictionary<Guid, ProductDto>();
            }

            var result = apiResponse.Data.ToDictionary(p => p.Id, p => p);
            
            logger.LogDebug("Successfully fetched {Count} products", result.Count);
            
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching products in batch");
            return new Dictionary<Guid, ProductDto>();
        }
    }
}