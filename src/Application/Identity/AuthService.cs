using Application.Common.Interfaces;

using Microsoft.AspNetCore.Identity;

using Persistence.Identity;

namespace Application.Identity;
public class AuthService
{
    private readonly CustomUserManager _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IdentityErrorDescriber _errorDescriber;
    private readonly IEmailSender _emailSender;

    public AuthService(CustomUserManager userManager, SignInManager<User> signInManager, IdentityErrorDescriber errorDescriber, IEmailSender emailSender)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _errorDescriber = errorDescriber;
        _emailSender = emailSender;
    }

    public async Task<(IdentityResult, string?)> RegisterAsync(string email, string password, string confirmPassword, string roleName)
    {
        if (password != confirmPassword)
        {
            return (IdentityResult.Failed(_errorDescriber.PasswordMismatch()), null);
        }
        var user = new User(email);

        //creates and adds user to role in a transaction
        var result = await _userManager.CreateAndAddToRoleTransactionAsync(user, password, roleName);
        if (result.Succeeded)
        {
            // Send email verification link to user's email
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            // Email sending logic
            await _emailSender.SendEmailAsync(user.Email!, "Confirm your email", $"Please confirm your account by clicking this link: {token}");
        }
        return (result, user.Id.ToString());
    }

    public async Task<IdentityResult> ConfirmEmailAsync(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return IdentityResult.Failed(_errorDescriber.InvalidToken());
        }
        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result;
    }

    public async Task<SignInResult> LoginAsync(string email, string password, bool isPersistent = true)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            return SignInResult.Failed;
        }

        if (!await _userManager.IsEmailConfirmedAsync(user))
        {
            return SignInResult.NotAllowed;
        }

        SignInResult result = await _signInManager.PasswordSignInAsync(email, password, isPersistent, lockoutOnFailure: false);

        return result;
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}
