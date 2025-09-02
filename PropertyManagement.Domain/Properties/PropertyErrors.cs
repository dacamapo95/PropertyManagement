using PropertyManagement.Shared.Results;

namespace PropertyManagement.Domain.Properties;

public static class PropertyErrors
{
    public static Error PropertyNotFound(Guid id) => Error.NotFound($"Property '{id}' not found.");
    public static Error OwnerNotFound(Guid id) => Error.NotFound($"Owner '{id}' not found.");
    public static Error CountryNotFound(Guid id) => Error.NotFound($"Country '{id}' not found.");
    public static Error StateNotFound(Guid id) => Error.NotFound($"State '{id}' not found.");
    public static Error CityNotFound(Guid id) => Error.NotFound($"City '{id}' not found.");
}
