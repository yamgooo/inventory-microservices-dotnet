using MediatR;
using ProductService.Application.DTOs;
using Shared.Common.Models;

namespace ProductService.Application.Queries.GetProducts;

public record GetProductsQuery(
    int Page = 1,
    int PageSize = 10
) : IRequest<Result<PagedResult<ProductDto>>>;