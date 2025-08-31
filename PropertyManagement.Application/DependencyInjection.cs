using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PropertyManagement.Application.Core.Behaviours;

namespace PropertyManagement.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(configure =>
        {
            configure.RegisterServicesFromAssembly(AssemblyReference.Assembly);
            configure.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            configure.AddBehavior(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(AssemblyReference.Assembly);

        return services;
    }
}