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
                "Product {ProductId} fetched successfully. Stock: {Stock}", 
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
                ProductId = productId,
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

            logger.LogInformation("Stock updated successfully for product {ProductId}", productId);
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
}