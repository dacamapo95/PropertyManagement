using PropertyManagement.Shared.Primitives;

namespace PropertyManagement.Domain.Owners;

public sealed class IdentificationType : MasterEntity<int>
{
    public ICollection<Owner> Owners { get; set; } = [];    
}