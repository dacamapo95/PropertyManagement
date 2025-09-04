using PropertyManagement.Application.Core.Abstractions;
using PropertyManagement.Domain.Countries;
using PropertyManagement.Shared.Results;

namespace PropertyManagement.Application.Features.Countries.GetCities;

public sealed class GetCitiesByStateIdQueryHandler(ICityRepository cityRepository)
    : IQueryHandler<GetCitiesByStateIdQuery, IReadOnlyList<CityResponse>>
{
    private readonly ICityRepository _cityRepository = cityRepository;

    public async Task<Result<IReadOnlyList<CityResponse>>> Handle(GetCitiesByStateIdQuery request, CancellationToken cancellationToken)
    {
        _cityRepository.DisableTracking();

        var cities = await _cityRepository.GetByStateIdAsync(request.StateId, cancellationToken);

        if (cities.Count == 0)
            return CountryErrors.CitiesNotFound(request.StateId);

        return cities.Select(c => new CityResponse(c.Id, c.Name, c.StateId))
                         .OrderBy(c => c.Name)
                         .ToList();
    }
}
