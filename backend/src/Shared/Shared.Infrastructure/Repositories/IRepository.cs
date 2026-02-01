// -------------------------------------------------------------
// By: Erik Portilla
// Date: 2026-01-29
// -------------------------------------------------------------

using Shared.Common.Models;

namespace Shared.Infrastructure.Repositories;

public interface IRepository<T> where T : class
{
    IQueryable<T> Table { get; }
    IQueryable<T> TableAsNoTracking { get; }
    
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<T>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity,  CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
