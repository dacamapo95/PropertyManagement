using PropertyManagement.Application.Core.Abstractions;
using PropertyManagement.Domain.Properties;
using PropertyManagement.Shared.Results;

namespace PropertyManagement.Application.Features.Properties.GetById;

public sealed class GetPropertyByIdQueryHandler(IPropertyRepository repo)
    : IQueryHandler<GetPropertyByIdQuery, PropertyResponse>
{
    private readonly IPropertyRepository _repo = repo;

    public async Task<Result<PropertyResponse>> Handle(GetPropertyByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repo.GetDetailsByIdAsync(request.Id, cancellationToken);

        if (entity is null)
            return Error.NotFound("Property not found.");

        return entity.ToPropertyResponse();
    }
}
