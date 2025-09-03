using PropertyManagement.Application.Core.Abstractions;
using PropertyManagement.Shared.Pagination;

namespace PropertyManagement.Application.Features.Properties.Get;

public sealed record GetPropertiesQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string? Search = null,
    int? StatusId = null,
    Guid? CityId = null,
    string? OrderBy = null,
    bool Desc = false
) : IQuery<PagedResult<PropertyListItem>>;

public sealed record PropertyListItem(
    Guid Id,
    string Name,
    string Address,
    decimal Price,
    int CodeInternal,
    int Year,
    int StatusId,
    string Status,
    Guid CityId,
    string City,
    Guid OwnerId,
    string Owner
);
