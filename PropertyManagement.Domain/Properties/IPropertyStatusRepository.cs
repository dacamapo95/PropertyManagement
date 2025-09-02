using PropertyManagement.Domain.Interfaces;

namespace PropertyManagement.Domain.Properties;

public interface IPropertyStatusRepository : IReadRepository<PropertyStatus, int>
{
}
