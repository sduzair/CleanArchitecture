using System.Security.Claims;

using Microsoft.AspNetCore.Identity;

namespace Application.Auth;
public interface IApplicationUserService
{
    Task<IdentityResult> DeleteUserAsync(ClaimsPrincipal claimsPrinciple);
    Task<IdentityResult> DeleteUserAsync(string email);
}
