using MediatR;
using ProductService.Application.Dto;
using Shared.Common.Models;

namespace ProductService.Application.Queries.GetProductsFiltered;

public record GetProductsFilteredQuery(
    string? Name,
    string? Category,
    decimal? MinPrice,
    decimal? MaxPrice,
    int? MinStock,
    int? MaxStock,
    int Page = 1,
    int PageSize = 10
) : IRequest<Result<PagedResult<ProductDto>>>;