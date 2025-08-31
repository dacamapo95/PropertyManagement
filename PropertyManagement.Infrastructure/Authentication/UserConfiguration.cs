using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace PropertyManagement.Infrastructure.Authentication;
public static class UsersConfiguration
{
    private const string defSchema = "AUTH";

    public static void ConfigureUsersContraints(ModelBuilder builder)
    {
        builder.Entity<User>(entity =>
        {
            entity.ToTable("Users", defSchema);

            entity.HasMany(e => e.UserRoles)
                    .WithOne(e => e.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();

            entity.HasMany(e => e.Logins)
                    .WithOne(e => e.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();

            entity.HasMany(e => e.Tokens)
                    .WithOne(e => e.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
        });

        builder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles", defSchema);

            entity.HasMany(e => e.UserRoles)
                    .WithOne(e => e.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();
        });

        builder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UserRoles", defSchema);
            entity.HasKey(key => new { key.UserId, key.RoleId });
        });

        builder.Entity<IdentityUserClaim<Guid>>(entity =>
        {
            entity.ToTable("UserClaims", defSchema);
        });

        builder.Entity<UserLogin>(entity =>
        {
            entity.ToTable("UserLogins", defSchema);
            entity.HasKey(key => new { key.ProviderKey, key.LoginProvider });
        });

        builder.Entity<IdentityRoleClaim<Guid>>(entity =>
        {
            entity.ToTable("RoleClaims", defSchema);
        });

        builder.Entity<UserToken>(entity =>
        {
            entity.ToTable("UserTokens", defSchema);
            entity.HasKey(key => new { key.UserId, key.LoginProvider, key.Name });
        });
    }
}
