using MediatR;
using Shared.Common.Models;

namespace ProductService.Application.Commands.DeleteProduct;

public record DeleteProductCommand(Guid Id) : IRequest<Result>;