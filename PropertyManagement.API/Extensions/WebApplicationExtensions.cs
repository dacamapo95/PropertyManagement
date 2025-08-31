using Carter;
using Serilog;

namespace PropertyManagement.API.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UsePipeline(this WebApplication app)
    {
        // Serilog request logging
        app.UseSerilogRequestLogging();

        app.UseExceptionHandler();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();
        app.MapCarter();

        return app;
    }
}