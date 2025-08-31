using PropertyManagement.Domain.Countries;

namespace PropertyManagement.Infrastructure.Database.Repositories;

public class CityReadRepository(ApplicationDbContext context)
    : Repository<City, Guid>(context), ICityReadRepository
{
}
