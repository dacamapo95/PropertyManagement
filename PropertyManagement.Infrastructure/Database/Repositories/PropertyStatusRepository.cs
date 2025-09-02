using PropertyManagement.Domain.Properties;

namespace PropertyManagement.Infrastructure.Database.Repositories;

public sealed class PropertyStatusRepository(ApplicationDbContext context)
    : Repository<PropertyStatus, int>(context), IPropertyStatusRepository
{
}
