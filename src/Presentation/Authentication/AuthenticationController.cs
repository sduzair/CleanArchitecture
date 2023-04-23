using System.Security.Claims;

using Application.Auth;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Presentation.Contracts.Auth;

namespace Presentation.Authentication;

[ApiController]
[Route("api/[controller]/[action]")]
[TypeFilter(typeof(AuthExceptionHandlingFilter))]
[Authorize]
public class AuthController : ControllerBase
{
    private readonly IApplicationAuthService _authService;

    public AuthController(IApplicationAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost()]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterDto model)
    {
        var (result, userId) = await _authService.RegisterAsync(model.Email, model.Password, model.ConfirmPassword);
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
        var (signInResult, email) = await _authService.LoginAsync(model.Email, model.Password, model.RememberMe);
        if (signInResult.Succeeded)
        {
            return Ok(new { Message = "Login successful.", Email = email});
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
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto confirmEmailDto)
    {
        if (string.IsNullOrEmpty(confirmEmailDto.Token))
        {
            return BadRequest("Token is required.");
        }

        var result = await _authService.ConfirmEmailAsync(confirmEmailDto.Token);
        if (result.Succeeded)
        {
            return Ok(new { Message = "Email verification successful." });
        }

        return BadRequest(result.Errors);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser()
    {
        var result = await _authService.DeleteUserAsync(HttpContext.User);
        if (result.Succeeded)
        {
            await _authService.LogoutAsync();
            return Ok(new { Message = "User deleted successfully." });
        }
        return BadRequest(result.Errors);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> DeleteUserByEmail(string email)
    {
        var result = await _authService.DeleteUserAsync(email);
        if (result.Succeeded)
        {
            return Ok(new { Message = "User deleted successfully." });
        }
        return BadRequest(result.Errors);
    }

}
