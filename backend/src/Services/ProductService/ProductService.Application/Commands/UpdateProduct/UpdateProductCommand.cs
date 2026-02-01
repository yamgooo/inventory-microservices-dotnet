using MediatR;
using ProductService.Application.Dto;
using Shared.Common.Models;

namespace ProductService.Application.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    string Category,
    string ImageUrl,
    decimal Price,
    int Stock
) : IRequest<Result<ProductDto>>;