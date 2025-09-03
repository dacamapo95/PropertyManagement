using Microsoft.EntityFrameworkCore;
using PropertyManagement.Domain.Owners;

namespace PropertyManagement.Infrastructure.Database.Repositories;

public sealed class OwnerRepository(ApplicationDbContext context)
    : Repository<Owner, Guid>(context), IOwnerRepository
{
    public async Task<Owner?> GetByIdentificationAsync(int identificationTypeId, string identificationNumber, CancellationToken cancellationToken = default)
    {
        return await Find(o => o.IdentificationTypeId == identificationTypeId
                                      && o.IdentificationNumber == identificationNumber, cancellationToken);
    }
}
