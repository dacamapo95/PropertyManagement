using Microsoft.EntityFrameworkCore;
using PropertyManagement.Domain.Properties;

namespace PropertyManagement.Infrastructure.Database.Repositories;

public sealed class PropertyRepository(ApplicationDbContext context)
    : Repository<Property, Guid>(context), IPropertyRepository
{
    public async Task<Property?> GetDetailsByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Query
            .Include(p => p.Status)
            .Include(p => p.Country)
            .Include(p => p.State)
            .Include(p => p.City)
            .Include(p => p.Owner)
                .ThenInclude(o => o.OwnerImages)
                    .ThenInclude(oi => oi.File)
            .Include(p => p.Images)
                .ThenInclude(pi => pi.File)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }
}
