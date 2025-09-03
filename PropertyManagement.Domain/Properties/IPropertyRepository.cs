using PropertyManagement.Domain.Interfaces;

namespace PropertyManagement.Domain.Properties;

public interface IPropertyRepository : IRepository<Property, Guid>
{
    Task<Property?> GetDetailsByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
