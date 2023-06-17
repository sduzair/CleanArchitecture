using System.Security.Claims;

using Application.Common.Interfaces;
using Application.Identity;

using Microsoft.AspNetCore.Identity;

using Persistence.Identity;

namespace Application.Users;

public class UserService
{
    private readonly CustomUserManager _userManager;
    private readonly IdentityErrorDescriber _errorDescriber;
    private readonly IEmailSender _emailSender;
    private readonly ITimeProvider _timeProvider;

    public UserService(CustomUserManager userManager, IEmailSender emailSender, ITimeProvider timeProvider)
    {
        _userManager = userManager;
        _errorDescriber = new IdentityErrorDescriber();
        _emailSender = emailSender;
        _timeProvider = timeProvider;
    }

    public async Task<IdentityResult> DeleteUserAsync(ClaimsPrincipal claimsPrinciple)
    {
        var user = await _userManager.GetUserAsync(claimsPrinciple);
        if (user == null)
        {
            return IdentityResult.Failed(_errorDescriber.DefaultError());
        }
        IdentityResult result = await _userManager.DeleteAsync(user);

        if (result.Errors.Any())
        {
            return result;
        }

        await _emailSender.SendEmailAsync(user.Email!, $"Your account with email {user.Email} has been deleted", "Thanks");
        return result;
    }

    public async Task<IdentityResult> DeleteUserAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return IdentityResult.Failed(_errorDescriber.DefaultError());
        }
        IdentityResult result = await _userManager.DeleteAsync(user);

        if (result.Errors.Any())
        {
            return result;
        }

        await _emailSender.SendEmailAsync(user.Email!, $"Your account with email {user.Email} has been deleted", "Thanks");
        return result;
    }

    public async Task<IdentityResult> RemoveUserFromRoleAsync(string email, string roleName)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return IdentityResult.Failed(_errorDescriber.DefaultError());
        }
        IdentityResult result = await _userManager.RemoveFromRoleAsync(user, roleName);

        if (result.Errors.Any())
        {
            return result;
        }

        await _emailSender.SendEmailAsync(user.Email!, "Role Changes", $"Your account with email {user.Email} has been removed from role {roleName} at {_timeProvider.GetTime()}");

        return result;
    }

    public async Task<IdentityResult> AddUserToRoleAsync(string email, string roleName)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return IdentityResult.Failed(_errorDescriber.DefaultError());
        }
        //throws System.InvalidOperationException if role does not exist (not caught here)
        IdentityResult result = await _userManager.AddToRoleAsync(user, roleName);
        if (result.Errors.Any())
        {
            return result;
        }

        await _emailSender.SendEmailAsync(user.Email!, "Role Changes", $"Your account with email {user.Email} has been added to role {roleName} at {_timeProvider.GetTime()}");
        return result;
    }

    public async Task<(IdentityResult, IReadOnlyList<string>?)> GetRolesForUserAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return (IdentityResult.Failed(_errorDescriber.DefaultError()), null);
        }

        var roles = await _userManager.GetRolesAsync(user);
        return (IdentityResult.Success, roles.AsReadOnly());
    }

    public async Task<List<string>> GetUsersInRoleAsync(string roleName)
    {
        IList<User> users = await _userManager.GetUsersInRoleAsync(roleName);
        var userNames = users.Select(u => u.Email!).ToList();
        return userNames;
    }
}
