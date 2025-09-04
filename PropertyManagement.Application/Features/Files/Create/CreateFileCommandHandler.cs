using PropertyManagement.Application.Core.Abstractions;
using PropertyManagement.Domain.Files;
using PropertyManagement.Shared.Results;

namespace PropertyManagement.Application.Features.Files.Create;

public sealed class CreateFileCommandHandler(IFileRepository fileRepository, IUnitOfWork uow)
    : ICommandHandler<CreateFileCommand, Guid>
{
    private readonly IFileRepository _fileRepository = fileRepository;
    private readonly IUnitOfWork _uow = uow;

    public async Task<Result<Guid>> Handle(CreateFileCommand request, CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(request.FileName);

        var file = new Domain.Files.File
        {
            Name = $"{Guid.NewGuid()}{extension}",
            OriginalName = Path.GetFileNameWithoutExtension(request.FileName),
            MimeType = request.ContentType,
            Extension = extension,
            Size = request.Data.LongLength,
            Url = null,
            Content = request.Data
        };

        _fileRepository.Add(file);
        await _uow.SaveChangesAsync(cancellationToken);

        return file.Id;
    }
}
