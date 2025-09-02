using PropertyManagement.Domain.Properties;
using PropertyManagement.Shared.Primitives;

namespace PropertyManagement.Domain.Countries;

public sealed class City : MasterEntity<Guid>
{
    public Guid CountryId { get; set; }
    public Country Country { get; set; } = default!;

    public Guid StateId { get; set; }
    public State State { get; set; } = default!;

    public ICollection<Property> Properties { get; set; } = new List<Property>();
}