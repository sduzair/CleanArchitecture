using Application.Auth;

using Microsoft.AspNetCore.Authorization;
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
        return BadRequest(result.Errors);
    }
}
