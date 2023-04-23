using System.Security.Claims;

using FluentResults;

using Microsoft.AspNetCore.Identity;

namespace Application.Auth;

public interface IApplicationAuthService
{
    Task<(IdentityResult, string?)> RegisterAsync(string email, string password, string confirmPassword);
    Task<IdentityResult> ConfirmEmailAsync(string token);
    Task<(SignInResult, string?)> LoginAsync(string email, string password, bool rememberMe);
    Task LogoutAsync();
    Task<IdentityResult> DeleteUserAsync(ClaimsPrincipal claimsPrinciple);
    Task<IdentityResult> DeleteUserAsync(string email);
    // Add other authentication-related methods as needed, e.g., password reset, etc.
}
