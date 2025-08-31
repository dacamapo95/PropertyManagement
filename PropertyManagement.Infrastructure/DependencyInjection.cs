using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PropertyManagement.Application.Core.Abstractions;
using PropertyManagement.Infrastructure.Authentication;
using PropertyManagement.Infrastructure.Authentication.Interfaces;
using PropertyManagement.Infrastructure.Authentication.Services;
using PropertyManagement.Infrastructure.Authentication.Settings;
using PropertyManagement.Infrastructure.Database;
using PropertyManagement.Infrastructure.Database.Interceptors;
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

        services.AddSingleton<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();

        // Database initializer
        services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();

        return services;
    }
}
