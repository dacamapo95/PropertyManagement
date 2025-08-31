using PropertyManagement.Shared.Results;
using PropertyManagement.Infrastructure.Authentication.Contracts;

namespace PropertyManagement.Infrastructure.Authentication.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponse>> LoginAsync(string email, string password, CancellationToken ct = default);
    Task<Result<AuthResponse>> RefreshAsync(string refreshToken, CancellationToken ct = default);
}
