using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs;
using ProductService.Application.Mappings;
using ProductService.Domain.Interfaces;
using Shared.Common.Models;

namespace ProductService.Application.Commands.UpdateProduct;

public class UpdateProductCommandHandler(
    IProductRepository repository,
    ILogger<UpdateProductCommandHandler> logger)
    : IRequestHandler<UpdateProductCommand, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Updating product: {ProductId}", request.Id);

            var product = await repository.GetByIdAsync(request.Id, cancellationToken);
            if (product == null)
            {
                logger.LogWarning("Product not found: {ProductId}", request.Id);
                return Result<ProductDto>.Failure("Product not found");
            }

            if (product.Name != request.Name)
            {
                var existingProduct = await repository.GetByNameAsync(request.Name, cancellationToken);
                if (existingProduct != null && existingProduct.Id != request.Id)
                {
                    logger.LogWarning("Product name {ProductName} already exists", request.Name);
                    return Result<ProductDto>.Failure($"A product with the name '{request.Name}' already exists");
                }
            }

            product.Name = request.Name;
            product.Description = request.Description;
            product.Category = request.Category;
            product.ImageUrl = request.ImageUrl;
            product.Price = request.Price;
            product.Stock = request.Stock;
            product.UpdatedAt = DateTime.UtcNow;

            await repository.UpdateAsync(product, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Product updated successfully: {ProductId}", product.Id);

            return Result<ProductDto>.Success(product.ToDto());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating product: {ProductId}", request.Id);
            return Result<ProductDto>.Failure("An error occurred while updating the product");
        }
    }
}