using PropertyManagement.Application.Core.Abstractions;
using PropertyManagement.Domain.Countries;
using PropertyManagement.Shared.Results;

namespace PropertyManagement.Application.Features.Countries.Get;

public sealed class GetCountriesQueryHandler(ICountryRepository repo) : IQueryHandler<GetCountriesQuery, IReadOnlyList<CountryResponse>>
{
    private readonly ICountryRepository _repo = repo;

    public async Task<Result<IReadOnlyList<CountryResponse>>> Handle(GetCountriesQuery request, CancellationToken cancellationToken)
    {
        _repo.DisableTracking();
        var countries = await _repo.GetAllAsync(cancellationToken);

        var list = countries
            .Select(c => new CountryResponse(c.Id, c.Name, c.Iso2, c.Iso3, c.PhoneCode, c.CurrencyCode))
            .OrderBy(c => c.Name)
            .ToList();

        if (list.Count == 0)
            return CountryErrors.CountriesNotFound;

        return list;
    }
}
