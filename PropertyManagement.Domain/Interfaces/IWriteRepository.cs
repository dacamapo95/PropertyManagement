using PropertyManagement.Shared.Primitives;

namespace PropertyManagement.Domain.Interfaces;

/// <summary>
/// Defines write operations for a repository handling entities of type <typeparamref name="TEntity"/>.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TId">The type of the entity identifier.</typeparam>
public interface IWriteRepository<TEntity, TId> where TEntity : Entity<TId> where TId : IEquatable<TId>
{
    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    void Add(TEntity entity);

    /// <summary>
    /// Updates an existing entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    void Update(TEntity entity);

    /// <summary>
    /// Removes an entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    void Remove(TEntity entity);
}