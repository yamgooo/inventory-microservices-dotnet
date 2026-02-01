using MediatR;
using ProductService.Application.Dto;
using Shared.Common.Models;

namespace ProductService.Application.Queries.GetProductsBatch;

public record GetProductsBatchQuery(Guid[] ids
) : IRequest<Result<List<ProductDto>>>;