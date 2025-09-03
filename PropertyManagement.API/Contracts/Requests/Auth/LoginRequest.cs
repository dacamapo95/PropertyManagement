namespace PropertyManagement.API.Contracts.Requests.Auth;

public sealed record LoginRequest(string Email, string Password);
