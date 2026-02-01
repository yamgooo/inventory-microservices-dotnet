using MediatR;
using Shared.Common.Models;
using TransactionService.Application.Dto;
using TransactionService.Domain.Enums;

namespace TransactionService.Application.Queries.GetTransactionsFiltered;

public record GetTransactionsFilteredQuery(
    int Page,
    int PageSize,
    int? MinAmount,
    int? MaxAmount,
    Guid? ProductId,
    TransactionType? TransactionType,
    DateTime? StartDate,
    DateTime? EndDate) : IRequest<Result<PagedResult<TransactionDto>>>;