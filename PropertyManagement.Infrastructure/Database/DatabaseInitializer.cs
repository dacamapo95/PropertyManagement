using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PropertyManagement.Infrastructure.Authentication;

namespace PropertyManagement.Infrastructure.Database;

public sealed class DatabaseInitializer(UserManager<User> userManager, RoleManager<Role> roleManager, ApplicationDbContext db, ILogger<DatabaseInitializer> logger) : IDatabaseInitializer
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly RoleManager<Role> _roleManager = roleManager;
    private readonly ApplicationDbContext _db = db;
    private readonly ILogger<DatabaseInitializer> _logger = logger;

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        // Ensure database and schema
        await _db.Database.EnsureCreatedAsync(cancellationToken);

        // Create default user if it does not exist
        var email = "danielcami782@gmail.com";
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        if (user is null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                UserName = email,
                NormalizedEmail = email.ToUpperInvariant(),
                NormalizedUserName = email.ToUpperInvariant(),
                EmailConfirmed = true
            };

            // NOTE: set a default password; change as appropriate
            var result = await _userManager.CreateAsync(user, "P@ssw0rd!");
            if (!result.Succeeded)
            {
                var error = string.Join(", ", result.Errors.Select(e => $"{e.Code}:{e.Description}"));
                _logger.LogError("Failed to create default user: {Errors}", error);
                throw new InvalidOperationException($"Failed to create default user: {error}");
            }
            _logger.LogInformation("Default user {Email} created.", email);
        }
        else
        {
            _logger.LogInformation("Default user {Email} already exists.", email);
        }
    }
}
