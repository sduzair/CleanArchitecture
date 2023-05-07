using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;

namespace Application.Auth;

public interface IApplicationAuthorizationService
{
    Task<bool> IsInRoleAsync(string userId, string roleName);
    Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, string policyName);
}
