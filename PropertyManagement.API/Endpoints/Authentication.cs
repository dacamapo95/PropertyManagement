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
        var group = app.MapGroup("/auth").WithTags("Auth");

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
        .Produces<AuthResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}