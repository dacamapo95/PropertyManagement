namespace PropertyManagement.Shared.Primitives;

public interface IAuditEntity
{
    public DateTime CreatedAtUtc { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? LastModifiedAtUtc { get; set; }

    public string? LastModifiedBy { get; set; }
}
