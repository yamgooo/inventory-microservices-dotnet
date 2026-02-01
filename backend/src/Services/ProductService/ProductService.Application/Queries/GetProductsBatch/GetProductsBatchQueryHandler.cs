using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Application.Dto;
using ProductService.Application.Mappings;
using ProductService.Domain.Interfaces;
using Shared.Common.Models;

namespace ProductService.Application.Queries.GetProductsBatch;

public class GetProductsBatchQueryHandler(
    IProductRepository repository,
    ILogger<GetProductsBatchQueryHandler> logger)
    : IRequestHandler<GetProductsBatchQuery, Result<List<ProductDto>>>
{
    public async Task<Result<List<ProductDto>>> Handle(
        GetProductsBatchQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Listing products by ids batch");
            
            var query = repository.TableAsNoTracking
                .Where(p => request.ids.Contains(p.Id) && !p.IsDeleted);

            var list = await query.Select(p => p.ToDto()).ToListAsync(cancellationToken: cancellationToken);
            
            logger.LogInformation("Product list retrieved successfully");

            return Result<List<ProductDto>>.Success(list);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error listing products");
            return Result<List<ProductDto>>.Failure(
                "An error occurred while filtering products");
        }
    }
}