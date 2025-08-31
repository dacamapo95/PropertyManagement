namespace PropertyManagement.Shared.Primitives;

public abstract class AuditableEntity<TId> : Entity<TId>, IAuditEntity where TId : IEquatable<TId>
{
    public DateTime CreatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModifiedAtUtc { get; set; }
    public string? LastModifiedBy { get; set; }
}