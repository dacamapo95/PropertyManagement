using PropertyManagement.Domain.Properties;
using PropertyManagement.Shared.Primitives;

namespace PropertyManagement.Domain.Owners;

public sealed class Owner : AuditableEntity<Guid>
{
    public string Name { get; set; } = default!;
    public string? Address { get; set; }
    public DateOnly? BirthDate { get; set; }
    public int IdentificationTypeId { get; set; }
    public IdentificationType IdentificationType { get; set; } = default!;
    public string IdentificationNumber { get; set; } = default!;
    public ICollection<Property> Properties { get; set; } = [];
    public ICollection<OwnerImage> OwnerImages { get; set; } = [];

}