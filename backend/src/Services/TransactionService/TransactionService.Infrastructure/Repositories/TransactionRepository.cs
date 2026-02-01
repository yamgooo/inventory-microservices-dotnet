using Shared.Infrastructure.Repositories;
using TransactionService.Domain.Entities;
using TransactionService.Domain.Interfaces;
using TransactionService.Infrastructure.Data;

namespace TransactionService.Infrastructure.Repositories;

public class TransactionRepository(TransactionContext context) : BaseRepository<Transaction>(context), ITransactionRepository
{

    public Task<Transaction?> GetByProductId(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}