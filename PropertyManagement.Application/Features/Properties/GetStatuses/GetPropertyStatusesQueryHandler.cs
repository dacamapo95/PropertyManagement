using PropertyManagement.Application.Core.Abstractions;
using PropertyManagement.Domain.Properties;
using PropertyManagement.Shared.Results;

namespace PropertyManagement.Application.Features.Properties.GetStatuses;

public sealed class GetPropertyStatusesQueryHandler(IPropertyStatusRepository repo)
    : IQueryHandler<GetPropertyStatusesQuery, IReadOnlyList<PropertyStatusResponse>>
{
    private readonly IPropertyStatusRepository _repo = repo;

    public async Task<Result<IReadOnlyList<PropertyStatusResponse>>> Handle(GetPropertyStatusesQuery request, CancellationToken cancellationToken)
    {
        _repo.DisableTracking();
        var propertyStatuses = await _repo.GetAllAsync(cancellationToken);

        if (propertyStatuses.Count == 0)
            return Error.NotFound("No property statuses found.");

        return propertyStatuses
            .Select(x => new PropertyStatusResponse(x.Id, x.Name))
            .OrderBy(x => x.Name)
            .ToList();
    }
}