using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.Dto;
using ProductService.Application.Mappings;
using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;
using Shared.Common.Models;

namespace ProductService.Application.Commands.CreateProduct;

public class CreateProductCommandHandler(
    IProductRepository repository,
    ILogger<CreateProductCommandHandler> logger)
    : IRequestHandler<CreateProductCommand, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Creating product: {ProductName}", request.Name);

            var existingProduct = await repository.GetByNameAsync(request.Name, cancellationToken);
            if (existingProduct != null)
            {
                logger.LogWarning("Product with name {ProductName} already exists", request.Name);
                return Result<ProductDto>.Failure($"A product with the name '{request.Name}' already exists");
            }

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Category = request.Category,
                ImageUrl = request.ImageUrl,
                Price = request.Price,
                Stock = request.Stock,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await repository.AddAsync(product, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Product created successfully with ID: {ProductId}", product.Id);

            return Result<ProductDto>.Success(product.ToDto());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating product: {ProductName}", request.Name);
            return Result<ProductDto>.Failure("An error occurred while creating the product");
        }
    }
}