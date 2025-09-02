using DomainFile = PropertyManagement.Domain.Files.File;
using PropertyManagement.Domain.Files;

namespace PropertyManagement.Infrastructure.Database.Repositories;

public sealed class FileRepository(ApplicationDbContext context)
    : Repository<DomainFile, Guid>(context), IFileRepository
{
}
