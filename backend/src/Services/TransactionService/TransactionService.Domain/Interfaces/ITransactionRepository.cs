using Shared.Infrastructure.Repositories;
using TransactionService.Domain.Entities;

namespace TransactionService.Domain.Interfaces;

public interface ITransactionRepository : IRepository<Transaction>
{
    Task<Transaction?> GetByProductId(Guid id, CancellationToken cancellationToken = default);
}