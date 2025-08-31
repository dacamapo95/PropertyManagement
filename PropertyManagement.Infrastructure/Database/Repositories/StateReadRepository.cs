using PropertyManagement.Domain.Countries;

namespace PropertyManagement.Infrastructure.Database.Repositories;

public class StateReadRepository(ApplicationDbContext context)
    : Repository<State, Guid>(context), IStateReadRepository
{
}
