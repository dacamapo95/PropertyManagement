using PropertyManagement.Domain.Countries;

namespace PropertyManagement.Infrastructure.Database.Repositories;

public class CountryReadRepository(ApplicationDbContext context)
    : Repository<Country, Guid>(context), ICountryReadRepository
{
}
