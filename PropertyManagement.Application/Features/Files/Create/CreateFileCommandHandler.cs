using PropertyManagement.Application.Core.Abstractions;
using PropertyManagement.Domain.Files;
using PropertyManagement.Shared.Results;

namespace PropertyManagement.Application.Features.Files.Create;

public sealed class CreateFileCommandHandler(IFileRepository repo, IUnitOfWork uow)
    : ICommandHandler<CreateFileCommand, Guid>
{
    private readonly IFileRepository _repo = repo;
    private readonly IUnitOfWork _uow = uow;

    public async Task<Result<Guid>> Handle(CreateFileCommand request, CancellationToken cancellationToken)
    {
        // Validations are handled by FluentValidation (CreateFileCommandValidator)

        var ext = Path.GetExtension(request.FileName);

        var entity = new PropertyManagement.Domain.Files.File
        {
            Id = Guid.NewGuid(),
            Name = Path.GetFileNameWithoutExtension(request.FileName),
            OriginalName = request.FileName,
            MimeType = request.ContentType,
            Extension = ext,
            Size = request.Data.LongLength,
            Url = null,
            Content = request.Data,
            CreatedAtUtc = DateTime.UtcNow
        };

        _repo.Add(entity);
        await _uow.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
