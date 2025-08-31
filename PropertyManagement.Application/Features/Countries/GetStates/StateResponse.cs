namespace PropertyManagement.Application.Features.Countries.GetStates;

public sealed record StateResponse(Guid Id, string Name, string? Code, Guid CountryId);
