using System.Threading;
using System.Threading.Tasks;

namespace PropertyManagement.Infrastructure.Database.Interfaces;

public interface IDatabaseInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}
