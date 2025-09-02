using PropertyManagement.Application.Core.Abstractions;

namespace PropertyManagement.Application.Features.Files.Create;

public sealed record CreateFileCommand(string FileName, string ContentType, byte[] Data) : ICommand<Guid>;
