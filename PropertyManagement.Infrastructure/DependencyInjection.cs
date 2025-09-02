using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PropertyManagement.Application.Core.Abstractions;
using PropertyManagement.Infrastructure.Authentication;
using PropertyManagement.Infrastructure.Authentication.Interfaces;
using PropertyManagement.Infrastructure.Authentication.Services;
using PropertyManagement.Infrastructure.Authentication.Settings;
using PropertyManagement.Infrastructure.Caching;
using PropertyManagement.Infrastructure.Database;
using PropertyManagement.Infrastructure.Database.Interceptors;
using PropertyManagement.Infrastructure.Database.Interfaces;
using PropertyManagement.Domain.Countries;
using PropertyManagement.Infrastructure.Database.Repositories;
using Microsoft.Extensions.Caching.Memory;
using PropertyManagement.Domain.Owners;
using PropertyManagement.Domain.Properties;
using PropertyManagement.Domain.Files;

namespace PropertyManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var inMemorySqliteConnection = new Microsoft.Data.Sqlite.SqliteConnection("DataSource=:memory:");
        inMemorySqliteConnection.Open();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>

            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>())
                   .UseSqlite(inMemorySqliteConnection)
        );

        services.AddScoped<ISaveChangesInterceptor, AuditableEntitySaveChangesInterceptor>();

        services.AddScoped<IUnitOfWork>(sp =>
            sp.GetRequiredService<ApplicationDbContext>());

        services
           .AddIdentityCore<User>()
           .AddRoles<Role>()
           .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddOptions<JwtOptions>().Bind(configuration.GetSection("Jwt"));

        services.AddMemoryCache();
        services.AddOptions<CacheOptions>().Bind(configuration.GetSection("Cache"));

        services.AddSingleton<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();

        services.AddScoped<ICountryRepository, CountryRepository>();

        services.AddScoped<StateRepository>();
        services.AddScoped<CityRepository>();

        services.AddScoped<IStateRepository>(sp =>
            new CachedStateRepository(
                sp.GetRequiredService<StateRepository>(),
                sp.GetRequiredService<IMemoryCache>(),
                sp.GetRequiredService<IOptions<CacheOptions>>()));

        services.AddScoped<ICityRepository>(sp =>
            new CachedCityRepository(
                sp.GetRequiredService<CityRepository>(),
                sp.GetRequiredService<IMemoryCache>(),
                sp.GetRequiredService<IOptions<CacheOptions>>()));

        // New repositories
        services.AddScoped<IIdentificationTypeRepository, IdentificationTypeRepository>();
        services.AddScoped<IPropertyStatusRepository, PropertyStatusRepository>();
        services.AddScoped<IFileRepository, FileRepository>();

        return services;
    }
}
