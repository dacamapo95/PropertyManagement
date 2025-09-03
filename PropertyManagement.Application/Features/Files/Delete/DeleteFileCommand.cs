using PropertyManagement.Application.Core.Abstractions;

namespace PropertyManagement.Application.Features.Files.Delete;

public sealed record DeleteFileCommand(Guid FileId) : ICommand;
