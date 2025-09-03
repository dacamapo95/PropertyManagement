using PropertyManagement.Application.Core.Abstractions;
using PropertyManagement.Domain.Files;
using PropertyManagement.Shared.Results;

namespace PropertyManagement.Application.Features.Files.Create;

public sealed class CreateFileCommandHandler(IFileRepository repo, IUnitOfWork uow)
    : ICommandHandler<CreateFileCommand, Guid>
{
    private readonly IFileRepository _fileRepository = repo;
    private readonly IUnitOfWork _uow = uow;

    public async Task<Result<Guid>> Handle(CreateFileCommand request, CancellationToken cancellationToken)
    {
        var ext = Path.GetExtension(request.FileName);

        var entity = new Domain.Files.File
        {
            Name = $"{Guid.NewGuid()}{ext}",
            OriginalName = Path.GetFileNameWithoutExtension(request.FileName),
            MimeType = request.ContentType,
            Extension = ext,
            Size = request.Data.LongLength,
            Url = null,
            Content = request.Data
        };

        _fileRepository.Add(entity);
        await _uow.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
