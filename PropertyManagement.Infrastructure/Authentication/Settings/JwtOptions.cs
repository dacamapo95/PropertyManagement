namespace PropertyManagement.Infrastructure.Authentication.Settings;

public sealed record JwtOptions(
    string Issuer,
    string Audience,
    string SecretKey,
    int AccessTokenExpirationMinutes,
    int RefreshTokenExpirationMinutes
);
