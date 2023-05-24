using System.Security.Claims;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Common.Security.Schemes.Visitor;

/// <summary>
/// Unauthorized requests will be signed in as visitor.
/// </summary>
public class CustomAuthenticationHandler : CookieAuthenticationHandler
{
    public CustomAuthenticationHandler(IOptionsMonitor<CookieAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock) { }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var result = await base.HandleAuthenticateAsync();

        if (result.Succeeded)
        {
            return result;
        }

        //sign in user as visitor
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, nameof(Visitor)),
            new Claim(ClaimTypes.Role, nameof(Visitor))
        };

        var identity = new ClaimsIdentity(claims, CustomAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await Context.SignInAsync(CustomAuthenticationDefaults.AuthenticationScheme, principal);

        var ticket = new AuthenticationTicket(principal, CustomAuthenticationDefaults.AuthenticationScheme);

        return AuthenticateResult.Success(ticket);
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        //return 401 response instead of redirecting to login page
        Response.StatusCode = 401;
        return Task.CompletedTask;
    }

    protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        //return 403 response instead of redirecting to access denied page
        Response.StatusCode = 403;
        return Task.CompletedTask;
    }
}
