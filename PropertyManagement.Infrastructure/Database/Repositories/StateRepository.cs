using PropertyManagement.Domain.Countries;

namespace PropertyManagement.Infrastructure.Database.Repositories;

public class StateRepository(ApplicationDbContext context)
    : Repository<State, Guid>(context), IStateRepository
{
    public async Task<IReadOnlyList<State>> GetByCountryIdAsync(Guid countryId, CancellationToken cancellationToken = default)
    {
        return await GetAsync(s => s.CountryId == countryId, cancellationToken); 
    }
}