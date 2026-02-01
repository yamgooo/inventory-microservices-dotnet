using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.Dto;
using ProductService.Application.Mappings;
using ProductService.Domain.Interfaces;
using Shared.Common.Models;

namespace ProductService.Application.Queries.GetProductById;

public class GetProductByIdQueryHandler(
    IProductRepository repository,
    ILogger<GetProductByIdQueryHandler> logger)
    : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting product by ID: {ProductId}", request.Id);

            var product = await repository.GetByIdAsync(request.Id, cancellationToken);

            if (product != null && !product.IsDeleted) return Result<ProductDto>.Success(product.ToDto());
            logger.LogWarning("Product not found: {ProductId}", request.Id);
            return Result<ProductDto>.Failure("Product not found");

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting product by ID: {ProductId}", request.Id);
            return Result<ProductDto>.Failure("An error occurred while retrieving the product");
        }
    }
}