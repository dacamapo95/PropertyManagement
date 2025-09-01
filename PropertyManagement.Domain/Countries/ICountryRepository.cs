using PropertyManagement.Domain.Interfaces;

namespace PropertyManagement.Domain.Countries;

public interface ICountryRepository : IReadRepository<Country, Guid>
{
}
