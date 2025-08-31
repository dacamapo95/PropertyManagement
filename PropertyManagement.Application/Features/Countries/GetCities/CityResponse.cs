namespace PropertyManagement.Application.Features.Countries.GetCities;

public sealed record CityResponse(Guid Id, string Name, Guid CountryId, Guid StateId);
