using MediatR;
using Shared.Common.Models;
using TransactionService.Application.Dto;
using TransactionService.Domain.Enums;

namespace TransactionService.Application.Commands.UpdateTransaction;

public record UpdateTransactionCommand(
    Guid Id,
    int Quantity,
    decimal UnitPrice,
    string? Details
) : IRequest<Result<TransactionDto>>;