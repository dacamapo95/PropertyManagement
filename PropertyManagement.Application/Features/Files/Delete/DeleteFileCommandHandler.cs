using PropertyManagement.Application.Core.Abstractions;
using PropertyManagement.Domain.Files;
using PropertyManagement.Shared.Results;

namespace PropertyManagement.Application.Features.Files.Delete;

public sealed class DeleteFileCommandHandler(IFileRepository repo, IUnitOfWork uow)
    : ICommandHandler<DeleteFileCommand>
{
    private readonly IFileRepository _fileRepository = repo;
    private readonly IUnitOfWork _uow = uow;

    public async Task<Result> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
    {
        var file = await _fileRepository.GetByIdAsync(request.FileId, cancellationToken);
        if (file is null) return Error.NotFound("File not found.");

        _fileRepository.Remove(file);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
