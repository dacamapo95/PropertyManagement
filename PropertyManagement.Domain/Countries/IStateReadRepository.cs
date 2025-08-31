using PropertyManagement.Domain.Interfaces;

namespace PropertyManagement.Domain.Countries;

public interface IStateReadRepository : IReadRepository<State, Guid>
{
}
