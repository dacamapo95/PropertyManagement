using PropertyManagement.Shared.Primitives;

namespace PropertyManagement.Domain.Geography;

public sealed class Country : MasterEntity<Guid>
{
    public string Iso2 { get; set; } = default!; // ISO 3166-1 alpha-2
    public string Iso3 { get; set; } = default!; // ISO 3166-1 alpha-3
    public int? IsoNumeric { get; set; } // ISO 3166-1 numeric
    public string? PhoneCode { get; set; }
    public string? CurrencyCode { get; set; }

    public ICollection<State> States { get; set; } = new List<State>();
    public ICollection<City> Cities { get; set; } = new List<City>();
}
