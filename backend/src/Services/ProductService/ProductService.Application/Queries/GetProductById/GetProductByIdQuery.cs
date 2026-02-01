using MediatR;
using ProductService.Application.Dto;
using Shared.Common.Models;

namespace ProductService.Application.Queries.GetProductById;

public record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductDto>>;