using PropertyManagement.Shared.Primitives;

namespace PropertyManagement.Domain.Owners;
public class OwnerImage : AuditableEntity<Guid>
{
    public Guid OwnerId { get; set; }
    public Owner Owner { get; set; } = default!;
    public Guid FileId { get; set; }
    public Files.File File { get; set; } = default!;

}
