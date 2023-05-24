using Application.Common.Security.Policies;
using Application.UserManager;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Presentation.Common;

namespace Presentation.UserManager;

[Authorize(Policy = nameof(ApplicationUserManagementPolicy))]
public sealed class UserManagerController : ApiControllerBase
{
    private readonly IApplicationUserService _applicationUserService;

    public UserManagerController(IApplicationUserService applicationUserService)
    {
        _applicationUserService = applicationUserService;
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUserByEmail(string email)
    {
        var result = await _applicationUserService.DeleteUserAsync(email);
        if (result.Succeeded)
        {
            return Ok(new { Message = "User deleted successfully." });
        }
        return BadRequest(result.Errors.FirstOrDefault());
    }

    [HttpPost]
    public async Task<IActionResult> RemoveUserFromRole(string email, string roleName)
    {
        var result = await _applicationUserService.RemoveUserFromRoleAsync(email, roleName);
        if (result.Succeeded)
        {
            return Ok(new { Message = "User removed from role successfully." });
        }
        return BadRequest(result.Errors.FirstOrDefault());
    }

    [HttpPost]
    public async Task<IActionResult> AddUserToRole(string email, string roleName)
    {
        var result = await _applicationUserService.AddUserToRoleAsync(email, roleName);
        if (result.Succeeded)
        {
            return Ok(new { Message = "User added to role successfully." });
        }
        return BadRequest(result.Errors.FirstOrDefault());
    }

    [HttpGet]
    public async Task<IActionResult> GetRolesForUser(string email)
    {
        (IdentityResult result, IReadOnlyList<string>? roles) = await _applicationUserService.GetRolesForUserAsync(email);

        if(!result.Succeeded)
        {
            return BadRequest(result.Errors.FirstOrDefault());
        }

        return Ok(roles!);
    }

    [HttpGet]
    public async Task<IActionResult> GetUsersInRole(string roleName)
    {
        var users = await _applicationUserService.GetUsersInRoleAsync(roleName);
        return Ok(users);
    }
}
