using Application.UserManager;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Presentation.Utility;

namespace Presentation.UserManager;

[Authorize]
public sealed class UserManagerController : ApiControllerBase
{
    private readonly IApplicationUserService _applicationUserService;

    public UserManagerController(IApplicationUserService applicationUserService)
    {
        _applicationUserService = applicationUserService;
    }

    [HttpPost]
    [AllowAnonymous]
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
    [AllowAnonymous]
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
    [AllowAnonymous]
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
    [AllowAnonymous]
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
    [AllowAnonymous]
    public async Task<IActionResult> GetUsersInRole(string roleName)
    {
        var users = await _applicationUserService.GetUsersInRoleAsync(roleName);
        return Ok(users);
    }
}
