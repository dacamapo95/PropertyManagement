using Microsoft.AspNetCore.Identity;

namespace PropertyManagement.Infrastructure.Authentication;

public class UserRole : IdentityUserRole<Guid>
{
    public User User { get; set; }

    public Role Role { get; set; }
}
