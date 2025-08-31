namespace PropertyManagement.Infrastructure.Authentication.Contracts;

public sealed record AuthResponse(
    Guid UserId,
    string Email,
    string? UserName,
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc
);
