using PropertyManagement.Application.Core.Abstractions;
using PropertyManagement.Domain.Countries;
using PropertyManagement.Shared.Results;

namespace PropertyManagement.Application.Features.Countries.GetStates;

public sealed class GetStatesByCountryIdQueryHandler(IStateReadRepository repo)
    : IQueryHandler<GetStatesByCountryIdQuery, IReadOnlyList<StateResponse>>
{
    private readonly IStateReadRepository _repo = repo;

    public async Task<Result<IReadOnlyList<StateResponse>>> Handle(GetStatesByCountryIdQuery request, CancellationToken cancellationToken)
    {
        _repo.DisableTracking();
        var states = await _repo.GetAsync(s => s.CountryId == request.CountryId, cancellationToken);

        return states.Select(s => new StateResponse(s.Id, s.Name, s.Code, s.CountryId))
            .OrderBy(s => s.Name)
            .ToList(); 
    }
}
