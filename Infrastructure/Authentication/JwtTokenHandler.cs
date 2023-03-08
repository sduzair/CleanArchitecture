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
    private readonly IDateTimeProvider _dateTimeProvider;
    //private readonly ILogger<JwtTokenHandler> _logger;

    public JwtTokenHandler(
        IOptions<JwtTokenOptions> jwtTokenOptions,
        IDateTimeProvider dateTimeProvider)
        //ILogger<JwtTokenHandler> logger)
    {
        _jwtTokenOptions = jwtTokenOptions.Value;   //validated on startup
        _dateTimeProvider = dateTimeProvider;
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
            notBefore: _dateTimeProvider.UtcNow.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials);

        return jwt;
    }

    public static string WriteToken(JwtSecurityToken jwt)
    {
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}