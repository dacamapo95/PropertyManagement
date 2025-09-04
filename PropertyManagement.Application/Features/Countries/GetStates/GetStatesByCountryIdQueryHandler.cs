using PropertyManagement.Application.Core.Abstractions;
using PropertyManagement.Domain.Countries;
using PropertyManagement.Shared.Results;

namespace PropertyManagement.Application.Features.Countries.GetStates;

public sealed class GetStatesByCountryIdQueryHandler(IStateRepository repo)
    : IQueryHandler<GetStatesByCountryIdQuery, IReadOnlyList<StateResponse>>
{
    private readonly IStateRepository _repo = repo;

    public async Task<Result<IReadOnlyList<StateResponse>>> Handle(GetStatesByCountryIdQuery request, CancellationToken cancellationToken)
    {
        _repo.DisableTracking();
        var states = await _repo.GetByCountryIdAsync(request.CountryId, cancellationToken);

        if (states.Count == 0)
            return CountryErrors.StatesNotFound(request.CountryId);

        return states.Select(s => new StateResponse(s.Id, s.Name, s.Code, s.CountryId))
            .OrderBy(s => s.Name)
            .ToList();
    }
}
