using PropertyManagement.Shared.Primitives;

namespace PropertyManagement.Domain.Countries;

public sealed class State : MasterEntity<Guid>
{
    public Guid CountryId { get; set; }
    public Country Country { get; set; } = default!;

    public string? Code { get; set; }

    public ICollection<City> Cities { get; set; } = new List<City>();
}
