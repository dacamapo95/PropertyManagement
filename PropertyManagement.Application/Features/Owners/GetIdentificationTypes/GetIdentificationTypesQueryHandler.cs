using PropertyManagement.Application.Core.Abstractions;
using PropertyManagement.Domain.Owners;
using PropertyManagement.Shared.Results;

namespace PropertyManagement.Application.Features.Owners.GetIdentificationTypes;

public sealed class GetIdentificationTypesQueryHandler(IIdentificationTypeRepository repository)
    : IQueryHandler<GetIdentificationTypesQuery, IReadOnlyList<IdentificationTypeResponse>>
{
    private readonly IIdentificationTypeRepository _repository = repository;

    public async Task<Result<IReadOnlyList<IdentificationTypeResponse>>> Handle(GetIdentificationTypesQuery request, CancellationToken cancellationToken)
    {
        _repository.DisableTracking();
        var identificationTypes = await _repository.GetAllAsync(cancellationToken);

        if (identificationTypes.Count == 0)
            return Error.NotFound("No identification types found.");

        return identificationTypes
            .Select(x => new IdentificationTypeResponse(x.Id, x.Name))
            .OrderBy(x => x.Name)
            .ToList();
    }
}
