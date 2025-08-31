using PropertyManagement.Infrastructure.Authentication.Settings;

namespace PropertyManagement.Infrastructure.Authentication.Interfaces;

public interface ITokenService
{
    (string accessToken, DateTime expiresAtUtc) CreateAccessToken(User user, JwtOptions options);
    (string refreshToken, DateTime expiresAtUtc) CreateRefreshToken(JwtOptions options);
}