using Application.Auth;

using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;
internal class ApplicationAuthenticationService : IApplicationAuthenticationService
{
    private readonly ApplicationUserManager _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IdentityErrorDescriber _errorDescriber;
    //private readonly IEmailSender _emailSender;

    public ApplicationAuthenticationService(ApplicationUserManager userManager, SignInManager<ApplicationUser> signInManager, IdentityErrorDescriber errorDescriber)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _errorDescriber = errorDescriber;
        //_emailSender = emailService;
    }

    public async Task<(IdentityResult, string?)> RegisterAsync(string email, string password, string confirmPassword, string roleName)
    {
        if (password != confirmPassword)
        {
            return (IdentityResult.Failed(_errorDescriber.PasswordMismatch()), null);
        }
        var user = new ApplicationUser(email);

        //creates and adds user to role in a transaction
        var result = await _userManager.CreateAndAddToRoleTransactionAsync(user, password, roleName);
        //if (result.Succeeded)
        //{
        //    // Send email verification link to user's email
        //    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //    // Email sending logic
        //    await _emailSender.SendEmailAsync(user.Email, "Confirm your email", $"Please confirm your account by clicking this link: {token}");
        //}
        return (result, user.Id.ToString());
    }

    public async Task<IdentityResult> ConfirmEmailAsync(string email, string token)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return IdentityResult.Failed(_errorDescriber.DefaultError());
        }
        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result;
    }

    public async Task<SignInResult> LoginAsync(string email, string password, bool isPersistent = true, bool lockoutOnFailure = false)
    {

        SignInResult result = await _signInManager.PasswordSignInAsync(email, password, isPersistent, lockoutOnFailure: false);
        return result;

    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}
