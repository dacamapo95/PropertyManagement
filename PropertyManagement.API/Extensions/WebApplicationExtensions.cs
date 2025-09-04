using Carter;
using Serilog;

namespace PropertyManagement.API.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UsePipeline(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseSerilogRequestLogging();
        app.UseHttpsRedirection();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapCarter();

        return app;
    }
}