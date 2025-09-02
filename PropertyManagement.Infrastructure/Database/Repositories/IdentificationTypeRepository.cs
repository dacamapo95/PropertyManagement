using PropertyManagement.Domain.Interfaces;
using PropertyManagement.Domain.Owners;
using PropertyManagement.Infrastructure.Database.Repositories;

namespace PropertyManagement.Infrastructure.Database.Repositories;

public sealed class IdentificationTypeRepository(ApplicationDbContext context)
    : Repository<IdentificationType, int>(context), IIdentificationTypeRepository
{
}
