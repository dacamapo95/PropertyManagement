using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Application.Core.Abstractions;
using PropertyManagement.Infrastructure;
using PropertyManagement.Infrastructure.Authentication;
using PropertyManagement.Domain.Countries;
using PropertyManagement.Domain.Properties;
using PropertyManagement.Domain.Owners;

public class ApplicationDbContext
    : IdentityDbContext<User, Role, Guid, IdentityUserClaim<Guid>, UserRole, UserLogin, IdentityRoleClaim<Guid>, UserToken>,
    IUnitOfWork
{

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Country> Countries => Set<Country>();
    public DbSet<State> States => Set<State>();
    public DbSet<City> Cities => Set<City>();

    // Domain - Properties/Owners/Files
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<PropertyStatus> PropertyStatuses => Set<PropertyStatus>();
    public DbSet<PropertyImage> PropertyImages => Set<PropertyImage>();
    public DbSet<PropertyTrace> PropertyTraces => Set<PropertyTrace>();

    public DbSet<PropertyManagement.Domain.Files.File> Files => Set<PropertyManagement.Domain.Files.File>();

    public DbSet<Owner> Owners => Set<Owner>();
    public DbSet<OwnerImage> OwnerImages => Set<OwnerImage>();
    public DbSet<IdentificationType> IdentificationTypes => Set<IdentificationType>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("PTY");
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
        UsersConfiguration.ConfigureUsersContraints(builder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}