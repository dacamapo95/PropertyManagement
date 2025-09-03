using PropertyManagement.Application.Core.Abstractions;
using PropertyManagement.Domain.Properties;
using PropertyManagement.Shared.Pagination;
using PropertyManagement.Shared.Results;

namespace PropertyManagement.Application.Features.Properties.Get;

public sealed class ListPropertiesQueryHandler(IPropertyRepository propertyRepository)
    : IQueryHandler<GetPropertiesQuery, PagedResult<PropertyListItem>>
{
    private readonly IPropertyRepository _propertyRepository = propertyRepository;

    public async Task<Result<PagedResult<PropertyListItem>>> Handle(GetPropertiesQuery request, CancellationToken cancellationToken)
    {
        var (entities, total) = await _propertyRepository.SearchAsync(
            request.PageNumber,
            request.PageSize,
            request.Search,
            request.StatusId,
            request.CityId,
            request.OrderBy,
            request.Desc,
            cancellationToken);

        var items = entities.Select(p => new PropertyListItem(
            p.Id,
            p.Name,
            p.Address,
            p.Price,
            p.CodeInternal,
            p.Year,
            p.StatusId,
            p.Status.Name,
            p.CityId,
            p.City.Name,
            p.OwnerId,
            p.Owner.Name)).ToList();

        return new PagedResult<PropertyListItem>(items, total, request.PageNumber, request.PageSize);
    }
}
