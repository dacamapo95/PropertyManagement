namespace PropertyManagement.API.Contracts.Requests.Properties;

public sealed record OwnerRequest(
    int IdentificationTypeId,
    string IdentificationNumber,
    string Name,
    string? Address,
    DateOnly? BirthDate,
    IReadOnlyList<Guid> OwnerFileIds
);
