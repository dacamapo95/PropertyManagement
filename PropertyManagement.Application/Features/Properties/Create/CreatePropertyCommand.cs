using PropertyManagement.Application.Core.Abstractions;

namespace PropertyManagement.Application.Features.Properties.Create;

public sealed record OwnerCommand(
    int IdentificationTypeId,
    string IdentificationNumber,
    string Name,
    string? Address,
    DateOnly? BirthDate,
    IReadOnlyList<Guid> OwnerFileIds
);

public sealed record CreatePropertyCommand(
    string Name,
    string Address,
    decimal Price,
    int CodeInternal,
    int Year,
    int StatusId,
    Guid CityId,
    OwnerCommand Owner,
    IReadOnlyList<Guid> PropertyFileIds
) : ICommand<CreatePropertyResponse>;
