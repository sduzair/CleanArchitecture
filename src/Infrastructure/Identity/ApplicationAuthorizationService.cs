using System.Security.Claims;

using Application.Auth;

using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Identity;
internal class ApplicationAuthorizationService : IApplicationAuthorizationService
{
    private readonly ApplicationUserManager _userManager;
    private readonly IAuthorizationService _authorizationService;

    public ApplicationAuthorizationService(ApplicationUserManager userManager, IAuthorizationService authorizationService)
    {
        _userManager = userManager;
        _authorizationService = authorizationService;
    }

    public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, string policyName)
    {
        return _authorizationService.AuthorizeAsync(user, policyName);
    }

    public async Task<bool> IsInRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return false;
        }

        return await _userManager.IsInRoleAsync(user, roleName);
    }
}
