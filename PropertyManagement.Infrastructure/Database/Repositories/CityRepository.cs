using PropertyManagement.Domain.Countries;

namespace PropertyManagement.Infrastructure.Database.Repositories;

public class CityRepository(ApplicationDbContext context)
    : Repository<City, Guid>(context), ICityRepository
{
    public async Task<IReadOnlyList<City>> GetByStateIdAsync(Guid stateId, CancellationToken cancellationToken = default)
    {
       return await GetAsync(c => c.StateId == stateId, cancellationToken);
    }
}
