using MediatR;
using Shared.Common.Models;
using TransactionService.Application.Dto;
using TransactionService.Domain.Enums;
using TransactionService.Domain.Interfaces;

namespace TransactionService.Application.Queries.GetTransactionsFiltered;

public record GetTransactionsFilteredCommand(ITransactionRepository repository) : IRequestHandler<GetTransactionsFilteredQuery, Result<TransactionDto>>
{
    public Task<Result<TransactionDto>> Handle(GetTransactionsFilteredQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}