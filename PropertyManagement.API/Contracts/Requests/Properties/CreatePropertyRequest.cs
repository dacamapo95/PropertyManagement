namespace PropertyManagement.API.Contracts.Requests.Properties;

public sealed record CreatePropertyRequest(
    string Name,
    string Address,
    decimal Price,
    int CodeInternal,
    int Year,
    int StatusId,
    Guid CityId,
    OwnerRequest Owner,
    IReadOnlyList<Guid> PropertyFileIds
);
