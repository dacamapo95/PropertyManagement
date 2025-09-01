using PropertyManagement.Domain.Countries;

namespace PropertyManagement.Infrastructure.Database.Repositories;

public class CountryRepository(ApplicationDbContext context)
    : Repository<Country, Guid>(context), ICountryRepository
{
}
