namespace Presentation.Contracts.Auth;
public record LoginDto(
    string Email,
    string Password,
    bool RememberMe);

