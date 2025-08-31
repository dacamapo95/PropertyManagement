using PropertyManagement.Shared.Results;

namespace PropertyManagement.Infrastructure.Authentication;

public static class UserErrors
{
   public static Error UnAuthorized => Error.Unauthorized("Invalid credentials");
}
