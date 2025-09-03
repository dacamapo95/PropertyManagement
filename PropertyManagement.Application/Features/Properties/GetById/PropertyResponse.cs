namespace PropertyManagement.Application.Features.Properties.GetById;

public sealed record PropertyResponse(
    Guid Id,
    string Name,
    string Address,
    decimal Price,
    int CodeInternal,
    int Year,
    int StatusId,
    string StatusName,
    Guid CountryId,
    string CountryName,
    Guid StateId,
    string StateName,
    Guid CityId,
    string CityName,
    OwnerResponse Owner,
    IReadOnlyList<PropertyImageResponse> Images
);
