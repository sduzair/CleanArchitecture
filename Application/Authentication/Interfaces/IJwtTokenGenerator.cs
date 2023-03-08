namespace Application.Authentication.Interfaces;
public interface IJwtTokenGenerator
{
    public (string accessToken, string expiresAt) GenerateAccessToken(Guid userId, string email, IEnumerable<string> roles);
}
