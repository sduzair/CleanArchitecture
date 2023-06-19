namespace Presentation.Authentication;
public record LoginDto(
    string Email,
    string Password,
    bool RememberMe);

