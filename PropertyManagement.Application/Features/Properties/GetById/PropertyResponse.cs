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
    Guid CityId,
    string CityName,
    OwnerResponse Owner,
    IReadOnlyList<PropertyImageResponse> Images
);
