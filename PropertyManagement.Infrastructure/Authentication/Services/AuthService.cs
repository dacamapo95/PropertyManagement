using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PropertyManagement.Infrastructure.Authentication.Interfaces;
using PropertyManagement.Infrastructure.Authentication.Settings;
using PropertyManagement.Infrastructure.Authentication.Contracts;
using PropertyManagement.Shared.Results;
using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.Infrastructure.Authentication.Services;

public sealed class AuthService(
    UserManager<User> userManager,
    ITokenService tokenService,
    ApplicationDbContext db,
    IOptions<JwtOptions> jwtOptions) : IAuthService
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly ITokenService _tokenService = tokenService;
    private readonly ApplicationDbContext _db = db;
    private readonly JwtOptions _jwt = jwtOptions.Value;

    public async Task<Result<AuthResponse>> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Error.Validation("Email is required");
        }

        if (!new EmailAddressAttribute().IsValid(email))
        {
            return Error.Validation("Invalid email format");
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            return Error.Validation("Password is required");
        }

        var user = await _userManager.Users.Include(u => u.Tokens).FirstOrDefaultAsync(u => u.Email == email, ct);
        if (user is null)
        {
            return UserErrors.UnAuthorized;
        }

        var isValidPassword = await _userManager.CheckPasswordAsync(user, password);

        if (!isValidPassword)
        {
            return UserErrors.UnAuthorized;
        }

        var (accessToken, accessExp) = _tokenService.CreateAccessToken(user, _jwt);
        var (refreshToken, refreshExp) = _tokenService.CreateRefreshToken(_jwt);

        user.SetRefreshToken(refreshToken, refreshExp);
        await _db.SaveChangesAsync(ct);

        var response = new AuthResponse(user.Id, user.Email!, user.UserName, accessToken, accessExp, refreshToken, refreshExp);
        return Result.Success(response);
    }

    public async Task<Result<AuthResponse>> RefreshAsync(string refreshToken, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Error.Validation("Refresh token is required");
        }

        var user = await _userManager.Users.Include(u => u.Tokens)
            .FirstOrDefaultAsync(u => u.Tokens.Any(t => t.Token == refreshToken), ct);
        if (user is null || !user.IsValidRefreshToken(refreshToken))
        {
            return Result.Fail<AuthResponse>(Error.Unauthorized("Invalid refresh token"));
        }

        var (accessToken, accessExp) = _tokenService.CreateAccessToken(user, _jwt);
        var (newRefresh, refreshExp) = _tokenService.CreateRefreshToken(_jwt);

        user.SetRefreshToken(newRefresh, refreshExp);
        await _db.SaveChangesAsync(ct);

        var response = new AuthResponse(user.Id, user.Email!, user.UserName, accessToken, accessExp, newRefresh, refreshExp);
        return Result.Success(response);
    }
}
