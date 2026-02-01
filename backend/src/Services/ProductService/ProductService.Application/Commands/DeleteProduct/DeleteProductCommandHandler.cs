using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Domain.Interfaces;
using Shared.Common.Models;

namespace ProductService.Application.Commands.DeleteProduct;

public class DeleteProductCommandHandler(
    IProductRepository repository,
    ILogger<DeleteProductCommandHandler> logger)
    : IRequestHandler<DeleteProductCommand, Result>
{
    public async Task<Result> Handle(
        DeleteProductCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Deleting product: {ProductId}", request.Id);

            var product = await repository.GetByIdAsync(request.Id, cancellationToken);
            if (product == null)
            {
                logger.LogWarning("Product not found: {ProductId}", request.Id);
                return Result.Failure("Product not found");
            }

            product.IsDeleted = true; // logic delete 
            product.UpdatedAt = DateTime.UtcNow;

            await repository.UpdateAsync(product, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Product deleted successfully: {ProductId}", request.Id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting product: {ProductId}", request.Id);
            return Result.Failure("An error occurred while deleting the product");
        }
    }
}