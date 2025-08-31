using Carter;
using PropertyManagement.API.Infrastructure;

namespace PropertyManagement.API.Extensions;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection RegisterApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer()
                .AddSwaggerGen()
                .AddProblemDetails()
                .AddExceptionHandler<GlobalExceptionHandler>()
                .AddCarter()
                .AddMemoryCache();


        return services;
    }
}