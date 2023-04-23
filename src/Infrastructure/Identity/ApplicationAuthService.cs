using System.Security.Claims;

using Application.Auth;

using Infrastructure.Common;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;
internal class ApplicationAuthService : IApplicationAuthService
{
    private readonly ApplicationUserManager _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    //private readonly IEmailSender _emailSender;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApplicationAuthService(ApplicationUserManager userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailService, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        //_emailSender = emailService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<(IdentityResult, string?)> RegisterAsync(string email, string password, string confirmPassword)
    {
        if (password != confirmPassword)
        {
            return (IdentityResult.Failed(new IdentityError { Description = "Passwords do not match" }), null);
        }
        var user = new ApplicationUser(email);
        var result = await _userManager.CreateAndAddToRoleTransactionAsync(user, password, "Admin");
        //if (result.Succeeded)
        //{
        //    // Send email verification link to user's email
        //    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //    // Email sending logic
        //    await _emailSender.SendEmailAsync(user.Email, "Confirm your email", $"Please confirm your account by clicking this link: {token}");
        //}
        return (result, user.Id.ToString());
    }

    public async Task<IdentityResult> ConfirmEmailAsync(string token)
    {
        string email = _httpContextAccessor.HttpContext!.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)!.Value;

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found" });
        }
        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result;
    }

    public async Task<(SignInResult, string?)> LoginAsync(string email, string password, bool rememberMe)
    {

        SignInResult result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);

        return (result, email);
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<IdentityResult> DeleteUserAsync(ClaimsPrincipal claimsPrinciple)
    {
        var user = await _userManager.GetUserAsync(claimsPrinciple);
        if (user != null)
        {
            return await _userManager.DeleteAsync(user);
        }
        return IdentityResult.Failed(new IdentityError { Description = "User not found" }); 
    }
    
    public async Task<IdentityResult> DeleteUserAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user != null)
        {
            return await _userManager.DeleteAsync(user);
        }
        return IdentityResult.Failed(new IdentityError { Description = "User not found" }); 
    }

    // Add other authentication-related methods as needed, e.g., password reset, etc.
}
