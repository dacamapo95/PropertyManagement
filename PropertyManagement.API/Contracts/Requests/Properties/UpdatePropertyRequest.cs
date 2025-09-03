namespace PropertyManagement.API.Contracts.Requests.Properties;

public sealed record UpdatePropertyRequest(
    string Name,
    string Address,
    int CodeInternal,
    int Year,
    Guid CityId,
    int StatusId,
    decimal? Price,
    decimal? Tax,
    OwnerRequest Owner,
    IReadOnlyList<Guid> PropertyFileIds
);
