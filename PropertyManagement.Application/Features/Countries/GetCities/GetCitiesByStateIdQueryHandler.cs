using PropertyManagement.Application.Core.Abstractions;
using PropertyManagement.Domain.Countries;
using PropertyManagement.Shared.Results;

namespace PropertyManagement.Application.Features.Countries.GetCities;

public sealed class GetCitiesByStateIdQueryHandler(ICityReadRepository repo)
    : IQueryHandler<GetCitiesByStateIdQuery, IReadOnlyList<CityResponse>>
{
    private readonly ICityReadRepository _repo = repo;

    public async Task<Result<IReadOnlyList<CityResponse>>> Handle(GetCitiesByStateIdQuery request, CancellationToken cancellationToken)
    {
        _repo.DisableTracking();

        var cities = await _repo.GetAsync(c => c.StateId == request.StateId, cancellationToken);

        return cities.Select(c => new CityResponse(c.Id, c.Name, c.CountryId, c.StateId))
                         .OrderBy(c => c.Name)
                         .ToList();
    }
}
