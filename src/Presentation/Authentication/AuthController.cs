using System.Security.Claims;

using Application.Auth;
using Application.Common.Security.Policies;
using Application.Common.Security.Requirements;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Presentation.Common;
using Presentation.Contracts.Auth;

namespace Presentation.Authentication;

[Authorize]
public sealed class AuthController : ApiControllerBase
{
    private readonly IApplicationAuthenticationService _authService;
    private static class SessionKeys
    {
        public const string UserId = "UserId";
    }

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
            SetUserIdInSession(userId);
            return Ok(new { Message = "Registration successful. Please check your email for verification.", UserId = userId });
        }

        return BadRequest(result.Errors);
    }

    [HttpPost()]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginDto model)
    {
        var signInResult = await _authService.LoginAsync(model.Email, model.Password, model.RememberMe);

        if (signInResult.Succeeded) {
            return Ok(new { Message = "Login successful.", model.Email});
        }
        else if (signInResult.IsLockedOut)
        {
            return BadRequest("User account locked out.");
        }
        else if (signInResult.IsNotAllowed)
        {
            return BadRequest("User account not allowed.");
        }
        else if (signInResult.RequiresTwoFactor)
        {
            return BadRequest("User account requires two factor authentication.");
        }

        return Unauthorized();
    }

    [HttpPost()]
    [Authorize(Policy = nameof(LogoutPolicy))]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        return Ok(new { Message = "Logout successful." });
    }

    [HttpGet()]
    public async Task<IActionResult> ConfirmEmail(string? id, string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return BadRequest("Token is required.");
        }

        id ??= GetUserIdFromSession();
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest("User Id is required.");
        }

        var result = await _authService.ConfirmEmailAsync(id, token);
        if (result.Succeeded)
        {
            return Ok(new { Message = "Email verification successful." });
        }

        return BadRequest(result.Errors);
    }

    private void SetUserIdInSession(string? userId)
    {
        HttpContext.Session.SetString(SessionKeys.UserId, userId!);
    }

    private string? GetUserIdFromSession()
    {
        return HttpContext.Session.GetString(SessionKeys.UserId);
    }
}
