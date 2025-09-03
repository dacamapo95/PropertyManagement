using PropertyManagement.Domain.Interfaces;

namespace PropertyManagement.Domain.Owners;

public interface IOwnerRepository : IRepository<Owner, Guid>
{
    Task<Owner?> GetByIdentificationAsync(int identificationTypeId, string identificationNumber, CancellationToken cancellationToken = default);
}
