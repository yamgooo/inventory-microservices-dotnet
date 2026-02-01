using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs;
using ProductService.Application.Mappings;
using ProductService.Domain.Interfaces;
using Shared.Common.Models;

namespace ProductService.Application.Queries.GetProducts;

public class GetProductsQueryHandler(
    IProductRepository repository,
    ILogger<GetProductsQueryHandler> logger)
    : IRequestHandler<GetProductsQuery, Result<PagedResult<ProductDto>>>
{
    public async Task<Result<PagedResult<ProductDto>>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting products - Page: {Page}, PageSize: {PageSize}", 
                request.Page, request.PageSize);

            var pagedResult = await repository.GetPagedAsync(
                request.Page, 
                request.PageSize, 
                cancellationToken);

            var dtos = pagedResult.Items.Select(p => p.ToDto()).ToList();

            var result = new PagedResult<ProductDto>
            {
                Items = dtos,
                TotalCount = pagedResult.TotalCount,
                Page = pagedResult.Page,
                PageSize = pagedResult.PageSize,
                TotalPages = pagedResult.TotalPages
            };

            return Result<PagedResult<ProductDto>>.Success(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting products");
            return Result<PagedResult<ProductDto>>.Failure("An error occurred while retrieving products");
        }
    }
}