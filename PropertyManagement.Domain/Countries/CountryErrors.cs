using PropertyManagement.Shared.Results;

namespace PropertyManagement.Domain.Countries;

public static class CountryErrors
{
    public static Error CountriesNotFound => Error.NotFound("No countries found.");

    public static Error StatesNotFound(Guid countryId) =>
        Error.NotFound($"No states found for country '{countryId}'.");

    public static Error CitiesNotFound(Guid stateId) =>
        Error.NotFound($"No cities found for state '{stateId}'.");
}
