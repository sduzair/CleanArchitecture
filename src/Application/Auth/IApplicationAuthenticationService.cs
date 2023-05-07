using Microsoft.AspNetCore.Identity;

namespace Application.Auth;

public interface IApplicationAuthenticationService
{
    Task<(IdentityResult, string?)> RegisterAsync(string email, string password, string confirmPassword, string roleName);
    Task<IdentityResult> ConfirmEmailAsync(string email, string token);
    Task LogoutAsync();
    Task<SignInResult> LoginAsync(string email, string password, bool rememberMe, bool lockoutOnFailure = false);
    // Add other authentication-related methods as needed, e.g., password reset, etc.
}
