using Application.Authentication.Interfaces;

using Domain.Entities;

namespace Application.Authentication;
internal sealed class AuthenticationService : IAuthenticationService
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    //private readonly IEmailService _emailService;
    private readonly IUserRepository<User> _userRepository;

    public AuthenticationService(IJwtTokenGenerator jwtTokenGenerator,
        IUserRepository<User> userRepository)
    //IEmailService emailService,
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _userRepository = userRepository;
        //_emailService = emailService;
    }

    public void ConfirmEmail(string email, string confirmationCode)
    {
        throw new NotImplementedException();
    }

    public async Task<AuthenticationResult> LoginAsync(string email, string password)
    {
        User user = await _userRepository.GetUserByEmail(email) ?? throw new Exceptions.UserNotFoundException();

        await VerifyPasswordAsync(email, password);
        (string accessToken, string expiresAt) = _jwtTokenGenerator.GenerateAccessToken(user.Id, email, roles: new[] { "user" });

        return new AuthenticationResult(
            AccessToken: accessToken,
            RefreshToken: "refresh token",
            AccessTokenExpiresAt: expiresAt,
            RefreshTokenExpiresAt: DateTime.Now.AddMinutes(1440).ToString("o"),
            UserId: user.Id);
    }

    private async Task VerifyPasswordAsync(string email, string password)
    {
        User user = await _userRepository.GetUserByEmail(email) ?? throw new Exceptions.UserNotFoundException();

        if(!user.Password.Equals(password))
        {
            throw new Exceptions.IncorrectPasswordException();
        }
    }

    public void Logout(string accessToken)
    {
        throw new NotImplementedException();
    }

    public async Task RegisterAsync(string email, string password, string confirmPassword, string? firstName, string? lastName, string? phoneNumber, string? addressLine1, string? addressLine2, string? city, string? state, string? zipCode, string? country)
    {
        if (await _userRepository.GetUserByEmail(email) is not null)
        {
            throw new Exceptions.UserExistsException();
        }

        var user = User.Create(email,
            password,
            confirmPassword);

        await _userRepository.AddAsync(user);

        return;
    }
}
