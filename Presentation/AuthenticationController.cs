
using Application.Authentication;

using Contracts.Authentication;

using Microsoft.AspNetCore.Mvc;

namespace Presentation;

[ApiController]
[Route("api/[controller]/[action]")]
//[ExceptionHandlingFilter]
public sealed class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost]
    public async Task<IActionResult> LoginAsync(LoginRequest loginRequest)
    {
        var authenticationResult = await _authenticationService.LoginAsync(
            loginRequest.Email,
            loginRequest.Password);

        AuthenticationResponse authenticationResponse = new(
            authenticationResult.AccessToken,
            authenticationResult.RefreshToken,
            authenticationResult.AccessTokenExpiresAt,
            authenticationResult.RefreshTokenExpiresAt,
            authenticationResult.UserId);
        
        return Ok(authenticationResponse);
    }

    //[HttpPost]
    //public IActionResult RefreshToken(RefreshTokenRequest refreshTokenRequest)
    //{
    //    var authenticationResult = _authenticationService.RefreshToken(
    //        refreshTokenRequest.AccessToken,
    //        refreshTokenRequest.RefreshToken);

    //    AuthenticationResponse authenticationResponse = new(
    //        authenticationResult.AccessToken,
    //        authenticationResult.RefreshToken,
    //        authenticationResult.AccessTokenExpiresAt,
    //        authenticationResult.RefreshTokenExpiresAt,
    //        authenticationResult.UserId);

    //    return Ok(authenticationResponse);
    //}

    [HttpPost]
    public IActionResult Logout(LogoutRequest logoutRequest)
    {
        _authenticationService.Logout(logoutRequest.AccessToken);

        return Ok();
    }

    //[HttpPost]
    //public IActionResult RevokeToken(RevokeTokenRequest revokeTokenRequest)
    //{
    //    _authenticationService.RevokeToken(
    //        revokeTokenRequest.AccessToken,
    //        revokeTokenRequest.RefreshToken);

    //    return Ok();
    //}

    [HttpPost]
    public async Task<IActionResult> RegisterAsync(RegisterRequest registerRequest)
    {
        await _authenticationService.RegisterAsync(
            registerRequest.Email,
            registerRequest.Password,
            registerRequest.ConfirmPassword,
            registerRequest.FirstName,
            registerRequest.LastName,
            registerRequest.PhoneNumber,
            registerRequest.AddressLine1,
            registerRequest.AddressLine2,
            registerRequest.City,
            registerRequest.State,
            registerRequest.ZipCode,
            registerRequest.Country);

        return Created("", new ProblemDetails
        {
            Title = "Registration successful",
            Detail = "Please check your email to confirm your account.",
            Status = 201
        });
    }

    [HttpPost]
    public IActionResult ConfirmEmail(ConfirmEmailRequest confirmEmailRequest)
    {
        _authenticationService.ConfirmEmail(
            confirmEmailRequest.Email,
            confirmEmailRequest.ConfirmationCode);

        return Ok();
    }

}
