namespace PropertyManagement.Infrastructure.Authentication.Settings;

public sealed class JwtOptions
{
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; init; }
    public int RefreshTokenExpirationMinutes { get; init; }
}
