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
            .Include(p => p.City)
            .Include(p => p.Owner)
                .ThenInclude(o => o.OwnerImages)
                    .ThenInclude(oi => oi.File)
            .Include(p => p.Images)
                .ThenInclude(pi => pi.File)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Property?> GetByCodeInternalAsync(int codeInternal, CancellationToken cancellationToken = default)
    {
        return await Find(p => p.CodeInternal == codeInternal, cancellationToken);
    }

    public async Task<(IReadOnlyList<Property> Items, int TotalCount)> SearchAsync(
        int pageNumber,
        int pageSize,
        string? search,
        int? statusId,
        Guid? cityId,
        string? orderBy,
        bool desc,
        CancellationToken cancellationToken = default)
    {
        var query = Query
            .Include(p => p.Status)
            .Include(p => p.City)
            .Include(p => p.Owner)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(p => p.Name.Contains(term) || p.Address.Contains(term));
        }

        if (statusId.HasValue)
            query = query.Where(p => p.StatusId == statusId.Value);

        if (cityId.HasValue)
            query = query.Where(p => p.CityId == cityId.Value);


        query = orderBy?.ToLowerInvariant() switch
        {
            "name" => desc ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
            "price" => desc ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
            "year" => desc ? query.OrderByDescending(p => p.Year) : query.OrderBy(p => p.Year),
            "codeinternal" => desc ? query.OrderByDescending(p => p.CodeInternal) : query.OrderBy(p => p.CodeInternal),
            "created" => desc ? query.OrderByDescending(p => p.CreatedAtUtc) : query.OrderBy(p => p.CreatedAtUtc),
            _ => query.OrderBy(p => p.Name)
        };

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}
