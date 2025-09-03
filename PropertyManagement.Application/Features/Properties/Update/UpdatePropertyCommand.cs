using PropertyManagement.Application.Core.Abstractions;

namespace PropertyManagement.Application.Features.Properties.Update;

public sealed record OwnerUpdate(
    int IdentificationTypeId,
    string IdentificationNumber,
    string Name,
    string? Address,
    DateOnly? BirthDate,
    IReadOnlyList<Guid> OwnerFileIds
);

public sealed record UpdatePropertyCommand(
    Guid Id,
    string Name,
    string Address,
    int CodeInternal,
    int Year,
    Guid CityId,
    int StatusId,
    decimal? Price,
    decimal? Tax,
    OwnerUpdate Owner,
    IReadOnlyList<Guid> PropertyFileIds
) : ICommand;
