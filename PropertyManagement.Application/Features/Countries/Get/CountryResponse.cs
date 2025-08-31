namespace PropertyManagement.Application.Features.Countries.Get;

public sealed record CountryResponse(Guid Id, string Name, string Iso2, string Iso3, string? PhoneCode, string? CurrencyCode);
