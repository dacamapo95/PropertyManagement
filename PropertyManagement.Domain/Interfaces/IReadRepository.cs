using PropertyManagement.Shared.Primitives;
using System.Linq.Expressions;

namespace PropertyManagement.Domain.Interfaces;

/// <summary>
/// Defines read-only repository operations for entities.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TId">The type of the entity identifier.</typeparam>
public interface IReadRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : IEquatable<TId>
{
    /// <summary>
    /// Disables change tracking for queries, improving performance for read-only operations.
    /// </summary>
    void DisableTracking();

    /// <summary>
    /// Asynchronously retrieves an entity by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The entity if found; otherwise, <c>null</c>.</returns>
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves all entities.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A read-only list of all entities.</returns>
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves entities matching the specified filter.
    /// </summary>
    /// <param name="filter">The filter expression to apply.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A read-only list of matching entities.</returns>
    Task<IReadOnlyList<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously determines whether any entities match the specified filter.
    /// </summary>
    /// <param name="filter">The filter expression to apply.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns><c>true</c> if any entities match the filter; otherwise, <c>false</c>.</returns>
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously counts the number of entities matching the specified filter.
    /// </summary>
    /// <param name="filter">The filter expression to apply. If <c>null</c>, counts all entities.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The number of matching entities.</returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default);
}