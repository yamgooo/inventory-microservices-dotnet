using MediatR;
using ProductService.Application.Dto;
using Shared.Common.Models;

namespace ProductService.Application.Commands.UpdateStock;

public record UpdateStockCommand(Guid Id, int Quantity) : IRequest<Result<ProductDto>>;