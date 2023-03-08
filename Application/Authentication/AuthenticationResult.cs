namespace Application.Authentication;
public record AuthenticationResult (
    string AccessToken,
    string RefreshToken,
    string AccessTokenExpiresAt,
    string RefreshTokenExpiresAt,
    Guid UserId);
