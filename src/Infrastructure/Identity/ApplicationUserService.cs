using System.Security.Claims;

using Application.UserManager;

using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

internal class ApplicationUserService : IApplicationUserService
{
    private readonly ApplicationUserManager _userManager;
    private readonly IdentityErrorDescriber _errorDescriber;

    public ApplicationUserService(ApplicationUserManager userManager, IdentityErrorDescriber errorDescriber)
    {
        _userManager = userManager;
        _errorDescriber = errorDescriber;
    }

    public async Task<IdentityResult> DeleteUserAsync(ClaimsPrincipal claimsPrinciple)
    {
        var user = await _userManager.GetUserAsync(claimsPrinciple);
        if (user == null)
        {
            return IdentityResult.Failed(_errorDescriber.DefaultError());
        }
        return await _userManager.DeleteAsync(user);
    }

    public async Task<IdentityResult> DeleteUserAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return IdentityResult.Failed(_errorDescriber.DefaultError());
        }
        return await _userManager.DeleteAsync(user);
    }

    public async Task<IdentityResult> RemoveUserFromRoleAsync(string email, string roleName)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return IdentityResult.Failed(_errorDescriber.DefaultError()); 
        }
        return await _userManager.RemoveFromRoleAsync(user, roleName);
    }

    public async Task<IdentityResult> AddUserToRoleAsync(string email, string roleName)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return IdentityResult.Failed(_errorDescriber.DefaultError());
        }
        //throws System.InvalidOperationException if role does not exist (not caught here)
        return await _userManager.AddToRoleAsync(user, roleName);
    }

    public async Task<(IdentityResult, IReadOnlyList<string>?)> GetRolesForUserAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if(user == null)
        {
            return (IdentityResult.Failed(_errorDescriber.DefaultError()), null);
        }
        return (IdentityResult.Success, (await _userManager.GetRolesAsync(user)).AsReadOnly());
    }

    public async Task<List<string>> GetUsersInRoleAsync(string roleName)
    {
        IList<ApplicationUser> users = await _userManager.GetUsersInRoleAsync(roleName);
        var userNames = users.Select(u => u.Email!).ToList();
        return userNames;
    }
}
