using System.Security.Claims;

using Application.Auth;

using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;
internal class ApplicationUserService : IApplicationUserService
{
    private readonly ApplicationUserManager _userManager;

    public ApplicationUserService(ApplicationUserManager userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityResult> DeleteUserAsync(ClaimsPrincipal claimsPrinciple)
    {
        var user = await _userManager.GetUserAsync(claimsPrinciple);
        if (user != null)
        {
            return await _userManager.DeleteAsync(user);
        }
        return IdentityResult.Failed(new IdentityError { Description = "ApplicationUser not found" }); 
    }
    
    public async Task<IdentityResult> DeleteUserAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user != null)
        {
            return await _userManager.DeleteAsync(user);
        }
        return IdentityResult.Failed(new IdentityError { Description = "ApplicationUser not found" }); 
    }
}
