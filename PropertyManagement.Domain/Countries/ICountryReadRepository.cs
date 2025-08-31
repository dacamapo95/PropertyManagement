using PropertyManagement.Domain.Interfaces;

namespace PropertyManagement.Domain.Countries;

public interface ICountryReadRepository : IReadRepository<Country, Guid>
{
}
