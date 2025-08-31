using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Application.Core.Abstractions;
using PropertyManagement.Infrastructure;
using PropertyManagement.Infrastructure.Authentication;

public class ApplicationDbContext
    : IdentityDbContext<User, Role, Guid, IdentityUserClaim<Guid>, UserRole, UserLogin, IdentityRoleClaim<Guid>, UserToken>,
    IUnitOfWork
{

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("PTY");
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}