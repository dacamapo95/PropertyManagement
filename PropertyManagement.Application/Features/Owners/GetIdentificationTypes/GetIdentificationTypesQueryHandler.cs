using PropertyManagement.Application.Core.Abstractions;
using PropertyManagement.Domain.Owners;
using PropertyManagement.Shared.Results;

namespace PropertyManagement.Application.Features.Owners.GetIdentificationTypes;

public sealed class GetIdentificationTypesQueryHandler(IIdentificationTypeRepository repo)
    : IQueryHandler<GetIdentificationTypesQuery, IReadOnlyList<IdentificationTypeResponse>>
{
    private readonly IIdentificationTypeRepository _repo = repo;

    public async Task<Result<IReadOnlyList<IdentificationTypeResponse>>> Handle(GetIdentificationTypesQuery request, CancellationToken cancellationToken)
    {
        _repo.DisableTracking();
        var identificationTypes = await _repo.GetAllAsync(cancellationToken);

        if (identificationTypes.Count == 0)
            return Error.NotFound("No identification types found.");

        return identificationTypes
            .Select(x => new IdentificationTypeResponse(x.Id, x.Name))
            .OrderBy(x => x.Name)
            .ToList();
    }
}
