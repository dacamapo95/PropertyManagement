using Microsoft.EntityFrameworkCore;
using PropertyManagement.Shared.Primitives;
using System.Linq.Expressions;

namespace PropertyManagement.Infrastructure.Database.Repositories;

public class Repository<TEntity, TId>(
    ApplicationDbContext context)
    where TEntity : Entity<TId> where TId : IEquatable<TId>
{
    protected readonly ApplicationDbContext _context = context;

    private bool _isTrackingDisabled;


    public virtual void Add(TEntity entity)
    {
        _context.Add(entity);
    }

    public virtual void Update(TEntity entity)
    {
        _context.Update(entity);
    }
    
    public virtual void Remove(TEntity entity)
    {
        _context.Remove(entity);
    }

    protected IQueryable<TEntity> Query => _context.Set<TEntity>();

    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await _context.AddRangeAsync(entities, cancellationToken);
    }

    public virtual void RemoveRange(IEnumerable<TEntity> entities)
    {
        _context.RemoveRange(entities);
    }

    public async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<TEntity>().FindAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await ConfigureQuery(null).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await ConfigureQuery(filter).ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await ConfigureQuery(filter).AnyAsync(filter, cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default)
    { 
        return await ConfigureQuery(filter).CountAsync(cancellationToken);
    }

    public void DisableTracking()
    {
        _isTrackingDisabled = true;
    }

    private IQueryable<TEntity> ConfigureQuery(Expression<Func<TEntity, bool>>? predicate)
    {
        var query = _context.Set<TEntity>().AsQueryable();
        if (_isTrackingDisabled) query = query.AsNoTracking();
        if (predicate != null) query = query.Where(predicate);
        return query;
    }
}