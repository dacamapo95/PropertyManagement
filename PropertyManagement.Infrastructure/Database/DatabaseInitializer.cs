using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PropertyManagement.Infrastructure.Authentication;
using PropertyManagement.Infrastructure.Database.Interfaces;
using System.Text.Json;
using PropertyManagement.Domain.Countries;
using PropertyManagement.Domain.Properties;
using PropertyManagement.Domain.Owners;

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
        await SeedMasterDataAsync(cancellationToken);
        await SeedUsGeographyAsync(cancellationToken);
    }

    private async Task EnsureDatabaseCreatedAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Applying database migrations...");
            await _db.Database.MigrateAsync(cancellationToken);
            _logger.LogInformation("Database migrations applied.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while applying database migrations.");
            throw;
        }
        
    }

    private async Task SeedDefaultUserAsync(CancellationToken cancellationToken)
    {
        const string email = "danielcami782@gmail.com";
        const string defaultPassword = "P@ssw0rd!";

        _logger.LogInformation("Seeding default user {Email} if needed...", email);

        var user = await _userManager.FindByEmailAsync(email);

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

    private async Task SeedMasterDataAsync(CancellationToken cancellationToken)
    {
        await SeedPropertyStatusesAsync(cancellationToken);
        await SeedIdentificationTypesAsync(cancellationToken);
    }

    private async Task SeedPropertyStatusesAsync(CancellationToken cancellationToken)
    {
        var statuses = Enum.GetValues<PropertyStatusEnum>()
            .Select(e => new PropertyStatus { Id = (int)e, Name = e.ToString() })
            .ToList();

        foreach (var status in statuses)
        {
            var exists = await _db.PropertyStatuses.AnyAsync(x => x.Id == status.Id, cancellationToken);
            if (!exists)
            {
                _db.PropertyStatuses.Add(status);
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedIdentificationTypesAsync(CancellationToken cancellationToken)
    {
        var types = Enum.GetValues<IdentificationTypeEnum>()
            .Select(e => new IdentificationType { Id = (int)e, Name = e.ToString() })
            .ToList();

        foreach (var t in types)
        {
            var exists = await _db.IdentificationTypes.AnyAsync(x => x.Id == t.Id, cancellationToken);
            if (!exists)
            {
                _db.IdentificationTypes.Add(t);
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedUsGeographyAsync(CancellationToken cancellationToken)
    {
        try
        {
            var country = await _db.Countries.FirstOrDefaultAsync(c => c.Iso2 == "US", cancellationToken);
            if (country is null)
            {
                country = new Country
                {
                    Id = Guid.NewGuid(),
                    Name = "United States",
                    Iso2 = "US",
                    Iso3 = "USA",
                    IsoNumeric = 840,
                    PhoneCode = "+1",
                    CurrencyCode = "USD"
                };
                _db.Countries.Add(country);
                await _db.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Seeded country {Name}", country.Name);
            }

            var jsonPath = Path.Combine(AppContext.BaseDirectory, "Database", "SeedData", "cities.json");
            if (!File.Exists(jsonPath))
            {
                _logger.LogWarning("cities.json not found at {Path}. Skipping US geography seeding.", jsonPath);
                return;
            }

            Dictionary<string, List<string>>? data;
            await using (var fs = File.OpenRead(jsonPath))
            {
                data = await JsonSerializer.DeserializeAsync<Dictionary<string, List<string>>>(fs, cancellationToken: cancellationToken);
            }

            if (data is null || data.Count == 0)
            {
                _logger.LogWarning("No states/cities found in cities.json");
                return;
            }

            foreach (var (stateNameRaw, cityListRaw) in data)
            {
                var stateName = stateNameRaw?.Trim();
                if (string.IsNullOrWhiteSpace(stateName)) continue;

                var state = await _db.States.FirstOrDefaultAsync(s => s.CountryId == country.Id && s.Name == stateName, cancellationToken);
                if (state is null)
                {
                    state = new State
                    {
                        Id = Guid.NewGuid(),
                        Name = stateName,
                        CountryId = country.Id
                    };
                    _db.States.Add(state);
                    await _db.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("Seeded state {Name}", stateName);
                }

                var cityNames = (cityListRaw ?? new List<string>())
                    .Where(n => !string.IsNullOrWhiteSpace(n))
                    .Select(n => n.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (cityNames.Count == 0) continue;

                var existingCityNames = await _db.Cities
                    .Where(c => c.StateId == state.Id)
                    .Select(c => c.Name)
                    .ToListAsync(cancellationToken);

                var toAdd = new List<City>();
                foreach (var cityName in cityNames)
                {
                    if (existingCityNames.Any(x => string.Equals(x, cityName, StringComparison.OrdinalIgnoreCase)))
                        continue;

                    toAdd.Add(new City
                    {
                        Id = Guid.NewGuid(),
                        Name = cityName,
                        StateId = state.Id
                    });
                }

                if (toAdd.Count > 0)
                {
                    _db.Cities.AddRange(toAdd);
                    await _db.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("Seeded {Count} cities for {State}", toAdd.Count, stateName);
                }
            }

            _logger.LogInformation("US geography seeding completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while seeding US geography data");
        }
    }
}
