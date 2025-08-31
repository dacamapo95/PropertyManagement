using Microsoft.AspNetCore.Identity;

namespace PropertyManagement.Infrastructure.Authentication;

public class Role : IdentityRole<Guid>
{
    public ICollection<UserRole> UserRoles { get; set; }
}