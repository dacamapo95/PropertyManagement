using PropertyManagement.Domain.Interfaces;

namespace PropertyManagement.Domain.Countries;

public interface ICityRepository : IReadRepository<City, Guid>
{
    Task<IReadOnlyList<City>> GetByStateIdAsync(Guid stateId, CancellationToken cancellationToken = default);
}
