using PropertyManagement.Shared.Primitives;

namespace PropertyManagement.Domain.Geography;

public sealed class State : MasterEntity<Guid>
{
    public Guid CountryId { get; set; }
    public Country Country { get; set; } = default!;
    public ICollection<City> Cities { get; set; } = new List<City>();
}
