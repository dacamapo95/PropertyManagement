using PropertyManagement.Shared.Primitives;

namespace PropertyManagement.Domain.Properties;

public sealed class PropertyStatus : MasterEntity<int>
{
    public ICollection<Property> Properties { get; set; } = [];
}
