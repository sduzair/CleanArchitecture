namespace Application.Authentication;

public interface IAuthenticationService
{
    public Task<AuthenticationResult > LoginAsync(string email, string password);
    public Task RegisterAsync(string email, string password, string confirmPassword, string? firstName, string? lastName, string? phoneNumber, string? addressLine1, string? addressLine2, string? city, string? state, string? zipCode, string? country);
    public void ConfirmEmail(string email, string confirmationCode);
    public void Logout(string accessToken);
}
