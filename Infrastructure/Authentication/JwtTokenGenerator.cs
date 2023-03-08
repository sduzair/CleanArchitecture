using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Application.Authentication.Interfaces;
using Application.Interfaces;

using Microsoft.Extensions.Options;

namespace Infrastructure.Authentication;
internal sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtTokenHandler _jwtTokenHandler;
    //private readonly IJwtTokenValidator _jwtTokenValidator;
    private readonly JwtTokenOptions _jwtTokenOptions;
    private readonly IDateTimeProvider _dateTimeProvider;
    //private readonly ILogger<JwtTokenGenerator> _logger;

    public JwtTokenGenerator(
        JwtTokenHandler jwtTokenHandler,
        //IJwtTokenValidator jwtTokenValidator,
        IOptions<JwtTokenOptions> jwtTokenOptions,
        IDateTimeProvider dateTimeProvider)
        //ILogger<JwtTokenGenerator> logger)
    {
        _jwtTokenHandler = jwtTokenHandler;
        //_jwtTokenValidator = jwtTokenValidator;
        _jwtTokenOptions = jwtTokenOptions.Value;   //validated on startup
        _dateTimeProvider = dateTimeProvider;
        //_logger = logger;
    }

    public (string accessToken, string expiresAt) GenerateAccessToken(Guid userId, string email, IEnumerable<string> roles)
    {
        DateTimeOffset now = _dateTimeProvider.UtcNow;
        DateTimeOffset expiresAt = now.AddMinutes(_jwtTokenOptions.AccessTokenExpirationMinutes);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new Claim(ClaimTypes.Role, string.Join(",", roles))
        };

        JwtSecurityToken jwt = _jwtTokenHandler.CreateToken(claims, expiresAt);
        string accessToken = JwtTokenHandler.WriteToken(jwt);

        return (accessToken, jwt.ValidTo.ToString("o"));
    }

}
