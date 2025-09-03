namespace PropertyManagement.Application.Features.Properties.GetById;

public sealed record OwnerResponse(
    Guid Id,
    string Name,
    string? Address,
    DateOnly? BirthDate,
    int IdentificationTypeId,
    string IdentificationNumber,
    IReadOnlyList<OwnerImageResponse> Images
);
