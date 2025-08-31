using Microsoft.AspNetCore.Identity;

namespace PropertyManagement.Infrastructure.Authentication;

public class UserLogin : IdentityUserLogin<Guid>
{
    public DateTime LoginDate { get; set; }

    public User User { get; set; }
}