namespace PropertyManagement.Application.Features.Properties.GetById;

public sealed record OwnerImageResponse(
    Guid FileId,
    string FileName,
    string ContentType,
    long Size,
    string Base64Data
);
