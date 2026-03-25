using System.Linq.Expressions;
using ERP.Shared;
using Microsoft.EntityFrameworkCore;

namespace ERP.Shared.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly DbContext _dbContext;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(DbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _dbSet = _dbContext.Set<T>();
    }

    public IQueryable<T> Query() => _dbSet.AsNoTracking();

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public void Update(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        _dbSet.Update(entity);
    }

    public void Remove(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        _dbSet.Remove(entity);
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }
}
