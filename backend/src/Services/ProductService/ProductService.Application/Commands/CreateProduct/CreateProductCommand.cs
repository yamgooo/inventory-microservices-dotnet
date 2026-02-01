using MediatR;
using ProductService.Application.Dto;
using Shared.Common.Models;

namespace ProductService.Application.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    string Description,
    string Category,
    string ImageUrl,
    decimal Price,
    int Stock
) : IRequest<Result<ProductDto>>;