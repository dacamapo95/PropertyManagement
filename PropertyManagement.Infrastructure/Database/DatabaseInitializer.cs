using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PropertyManagement.Infrastructure.Authentication;
using PropertyManagement.Infrastructure.Database.Interfaces;

namespace PropertyManagement.Infrastructure.Database;

public sealed class DatabaseInitializer(UserManager<User> userManager, RoleManager<Role> roleManager, ApplicationDbContext db, ILogger<DatabaseInitializer> logger) : IDatabaseInitializer
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly RoleManager<Role> _roleManager = roleManager;
    private readonly ApplicationDbContext _db = db;
    private readonly ILogger<DatabaseInitializer> _logger = logger;

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await EnsureDatabaseCreatedAsync(cancellationToken);
        await SeedDefaultUserAsync(cancellationToken);
    }

    private async Task EnsureDatabaseCreatedAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Ensuring database is created...");
        await _db.Database.EnsureCreatedAsync(cancellationToken);
        _logger.LogInformation("Database ensure created completed.");
    }

    private async Task SeedDefaultUserAsync(CancellationToken cancellationToken)
    {
        const string email = "danielcami782@gmail.com";
        const string defaultPassword = "P@ssw0rd!";

        _logger.LogInformation("Seeding default user {Email} if needed...", email);

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        if (user is not null)
        {
            _logger.LogInformation("Default user {Email} already exists.", email);
            return;
        }

        user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            UserName = email,
            NormalizedEmail = email.ToUpperInvariant(),
            NormalizedUserName = email.ToUpperInvariant(),
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, defaultPassword);

        if (!result.Succeeded)
        {
            var error = string.Join(", ", result.Errors.Select(e => $"{e.Code}:{e.Description}"));
            _logger.LogError("Failed to create default user: {Errors}", error);
            return;
        }

        _logger.LogInformation("Default user {Email} created.", email);
    }
}
