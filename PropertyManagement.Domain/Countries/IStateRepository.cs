using PropertyManagement.Domain.Interfaces;

namespace PropertyManagement.Domain.Countries;

public interface IStateRepository : IReadRepository<State, Guid>
{
    Task<IReadOnlyList<State>> GetByCountryIdAsync(Guid countryId, CancellationToken cancellationToken = default);
}
