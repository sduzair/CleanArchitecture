namespace Contracts.Authentication;
public record ConfirmEmailRequest(
    string Email,
    string ConfirmationCode);
