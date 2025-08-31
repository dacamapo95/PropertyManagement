using PropertyManagement.Application.Core.Abstractions;

namespace PropertyManagement.Application.Features.Countries.Get;

public sealed record GetCountriesQuery() : IQuery<IReadOnlyList<CountryResponse>>;
