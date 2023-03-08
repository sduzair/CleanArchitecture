namespace Contracts.Authentication;
public record AuthenticationResponse(
    string Token,
    string RefreshToken,
    string TokenExpires,
    string RefreshTokenExpires,
    Guid UserId);




