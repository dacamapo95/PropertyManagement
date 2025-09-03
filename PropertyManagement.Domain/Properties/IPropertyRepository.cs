using PropertyManagement.Domain.Interfaces;

namespace PropertyManagement.Domain.Properties;

public interface IPropertyRepository : IRepository<Property, Guid>
{
    Task<Property?> GetDetailsByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Property?> GetByCodeInternalAsync(int codeInternal, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Property> Items, int TotalCount)> SearchAsync(
        int pageNumber,
        int pageSize,
        string? search,
        int? statusId,
        Guid? cityId,
        string? orderBy,
        bool desc,
        CancellationToken cancellationToken = default);
}
