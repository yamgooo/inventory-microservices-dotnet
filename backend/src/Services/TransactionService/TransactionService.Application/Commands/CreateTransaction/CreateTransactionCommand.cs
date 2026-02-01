using MediatR;
using Shared.Common.Models;
using TransactionService.Application.Dto;
using TransactionService.Domain.Enums;

namespace TransactionService.Application.Commands.CreateTransaction;

public record CreateTransactionCommand(
    Guid ProductId,
    TransactionType Type,
    int Quantity,
    decimal UnitPrice,
    decimal? TotalPrice,
    string Details
    ) : IRequest<Result<TransactionDto>>;