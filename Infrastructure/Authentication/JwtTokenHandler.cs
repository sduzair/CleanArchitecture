using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Application.Interfaces;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Authentication;

internal sealed class JwtTokenHandler
{
    private readonly JwtTokenOptions _jwtTokenOptions;
    private readonly ITimeProvider _timeProvider;
    //private readonly ILogger<JwtTokenHandler> _logger;

    public JwtTokenHandler(
        IOptions<JwtTokenOptions> jwtTokenOptions,
        ITimeProvider timeProvider)
        //ILogger<JwtTokenHandler> logger)
    {
        _jwtTokenOptions = jwtTokenOptions.Value;   //validated on startup
        _timeProvider = timeProvider;
        //_logger = logger;
    }
    public JwtSecurityToken CreateToken(IEnumerable<Claim> claims, DateTimeOffset expiresAt)
    {
        byte[] key = Encoding.UTF8.GetBytes(_jwtTokenOptions.Secret.Reveal());
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            _jwtTokenOptions.Issuer,
            _jwtTokenOptions.Audience,
            claims,
            notBefore: _timeProvider.GetTime().UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials);

        return jwt;
    }

    public static string WriteToken(JwtSecurityToken jwt)
    {
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}