using Carter;
using Microsoft.AspNetCore.Mvc;
using PropertyManagement.API.Extensions;
using PropertyManagement.Infrastructure.Authentication.Interfaces;
using PropertyManagement.Infrastructure.Authentication.Contracts;
using PropertyManagement.API.Contracts.Requests.Auth;

namespace PropertyManagement.API.Endpoints;

public sealed class Authentication : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth").WithTags("Auth").WithOpenApi();

        group.MapPost("/login", async (
            [FromBody] LoginRequest request,
            IAuthService authService,
            CancellationToken ct) =>
        {
            var result = await authService.LoginAsync(request.Email, request.Password, ct);
            return result.IsValid
                ? Results.Ok(result.Value)
                : ResultExtension.ResultToResponse(result);
        })
        .WithName("Login")
        .WithSummary("Iniciar sesión en el sistema")
        .WithDescription("Permite a un usuario autenticarse en el sistema usando email y contraseña. Retorna un token de acceso y un token de actualización.")
        .Produces<AuthResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("/refresh", async (
            [FromBody] RefreshRequest request,
            IAuthService authService,
            CancellationToken ct) =>
        {
            var result = await authService.RefreshAsync(request.RefreshToken, ct);
            return result.IsValid
                ? Results.Ok(result.Value)
                : ResultExtension.ResultToResponse(result);
        })
        .WithName("RefreshToken")
        .WithSummary("Renovar token de acceso")
        .WithDescription("Renueva el token de acceso usando un token de actualización válido. Útil para mantener la sesión activa sin requerir credenciales nuevamente.")
        .Produces<AuthResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}