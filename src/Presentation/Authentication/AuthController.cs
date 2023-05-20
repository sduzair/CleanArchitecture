using System.Security.Claims;

using Application.Auth;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Presentation.Common;
using Presentation.Contracts.Auth;

namespace Presentation.Authentication;

[Authorize]
public sealed class AuthController : ApiControllerBase
{
    private readonly IApplicationAuthenticationService _authService;

    public AuthController(IApplicationAuthenticationService authService)
    {
        _authService = authService;
    }

    [HttpPost()]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterDto model)
    {
        var (result, userId) = await _authService.RegisterAsync(model.Email, model.Password, model.ConfirmPassword, model.RoleName);
        if (result.Succeeded)
        {
            return Ok(new { Message = "Registration successful. Please check your email for verification.", UserId = userId });
        }

        return BadRequest(result.Errors);
    }

    [HttpPost()]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginDto model)
    {
        var signInResult = await _authService.LoginAsync(model.Email, model.Password, model.RememberMe);
        if (signInResult.Succeeded)
        {
            return Ok(new { Message = "Login successful.", model.Email});
        }

        return Unauthorized();
    }

    [HttpPost()]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        return Ok(new { Message = "Logout successful." });
    }

    [HttpGet()]
    public async Task<IActionResult> ConfirmEmail(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return BadRequest("Token is required.");
        }

        string email = HttpContext!.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)!.Value;

        var result = await _authService.ConfirmEmailAsync(email, token);
        if (result.Succeeded)
        {
            return Ok(new { Message = "Email verification successful." });
        }

        return BadRequest(result.Errors);
    }
}
