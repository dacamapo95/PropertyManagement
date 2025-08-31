using Microsoft.AspNetCore.Identity;

namespace PropertyManagement.Infrastructure.Authentication;

public class UserToken : IdentityUserToken<Guid>
{
    public string? Token { get; set; }

    public DateTime? ExpiryTime { get; set; }

    public User User { get; set; }

    public UserToken(string? refreshToken, DateTime? expirationDate)
    {
        LoginProvider = "PropertyManagement";
        Name = "PropertyManagement";
        Token = refreshToken;
        ExpiryTime = expirationDate;
    }

    public UserToken()
    {

    }
}