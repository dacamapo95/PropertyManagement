using PropertyManagement.Application.Core.Abstractions;

namespace PropertyManagement.Application.Features.Files.GetById;

public sealed record GetFileByIdQuery(Guid Id) : IQuery<FileResponse>;
