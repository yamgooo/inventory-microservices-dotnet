using Microsoft.EntityFrameworkCore;
using Shared.Common.Models;

namespace Shared.Infrastructure.Repositories;

public abstract class BaseRepository<T>(DbContext context) : IRepository<T>
    where T : class
{
    protected readonly DbContext Context = context;
    protected readonly DbSet<T> Entities = context.Set<T>();

    public IQueryable<T> Table => Entities;
    public IQueryable<T> TableAsNoTracking => Entities.AsNoTracking();

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id cannot be empty", nameof(id));

        return await Entities.FindAsync([id], cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await Entities.AsNoTracking().ToListAsync(cancellationToken);

    public virtual async Task<PagedResult<T>> GetPagedAsync(
        int page, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        if (page < 1)
            throw new ArgumentException("Page must be greater than 0", nameof(page));

        if (pageSize is < 1 or > 100)
            throw new ArgumentException("PageSize must be between 1 and 100", nameof(pageSize));

        var totalCount = await Entities.CountAsync(cancellationToken);
 
        var items = await Entities
            .AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await Entities.AddAsync(entity, cancellationToken);
        return entity;
    }

    public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        
        var entry = Context.Entry(entity);
        
        if (entry.State == EntityState.Detached)
            Entities.Attach(entity);
        
        entry.State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        Entities.Remove(entity);
        return Task.CompletedTask;
    }

    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await Context.SaveChangesAsync(cancellationToken);
}