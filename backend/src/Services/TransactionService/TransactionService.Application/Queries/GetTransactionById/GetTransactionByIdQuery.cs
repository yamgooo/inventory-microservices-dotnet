using MediatR;
using Shared.Common.Models;
using TransactionService.Application.Dto;

namespace TransactionService.Application.Queries.GetTransactionById;

public record GetTransactionByIdQuery(Guid Id) : IRequest<Result<TransactionDto>>;