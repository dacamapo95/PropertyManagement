namespace PropertyManagement.Application.Features.Files.GetById;

public sealed record FileResponse(
    Guid Id,
    string Name, 
    string OriginalName, 
    string ContentType,
    string Extension,
    long Size,
    string? Url, 
    string Base64Data);
