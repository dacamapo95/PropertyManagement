using PropertyManagement.Application.Core.Abstractions;
using PropertyManagement.Domain.Countries;
using PropertyManagement.Shared.Results;

namespace PropertyManagement.Application.Features.Countries.GetCities;

public sealed class GetCitiesByStateIdQueryHandler(ICityRepository repo)
    : IQueryHandler<GetCitiesByStateIdQuery, IReadOnlyList<CityResponse>>
{
    private readonly ICityRepository _repo = repo;

    public async Task<Result<IReadOnlyList<CityResponse>>> Handle(GetCitiesByStateIdQuery request, CancellationToken cancellationToken)
    {
        _repo.DisableTracking();

        var cities = await _repo.GetByStateIdAsync(request.StateId, cancellationToken);

        var list = cities.Select(c => new CityResponse(c.Id, c.Name, c.StateId))
                         .OrderBy(c => c.Name)
                         .ToList();

        if (list.Count == 0)
            return CountryErrors.CitiesNotFound(request.StateId);

        return list;
    }
}
