using Microsoft.AspNetCore.Identity;
using PropertyManagement.Shared.Primitives;

namespace PropertyManagement.Infrastructure.Authentication;

public class User : IdentityUser<Guid>, IEntity<Guid>
{
    public ICollection<UserRole> UserRoles { get; set; } = [];

    public ICollection<UserLogin> Logins { get; set; } = [];

    public ICollection<UserToken> Tokens { get; set; } = [];

    public bool IsValidRefreshToken(string refreshToken)
    {
        var token = Tokens.FirstOrDefault(t => t.Token == refreshToken && t.ExpiryTime.HasValue);
        return token != null && token.ExpiryTime.HasValue && DateTime.UtcNow < token.ExpiryTime.Value;
    }

    public void SetRefreshToken(string refresh, DateTime expiryTime)
    {
        var token = Tokens.FirstOrDefault();

        if (token == null)
        {
            Tokens.Add(new UserToken(refresh, expiryTime));
        }
        else
        {
            token.Token = refresh;
            token.ExpiryTime = expiryTime;
        }
    }
}
