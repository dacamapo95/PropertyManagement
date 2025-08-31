using PropertyManagement.Application.Core.Abstractions;

namespace PropertyManagement.Application.Features.Countries.GetCities;

public sealed record GetCitiesByStateIdQuery(Guid StateId) : IQuery<IReadOnlyList<CityResponse>>;
