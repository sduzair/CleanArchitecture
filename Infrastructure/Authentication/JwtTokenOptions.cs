using System.ComponentModel.DataAnnotations;

using Infrastructure.Utilities;

namespace Infrastructure.Authentication;
internal sealed class JwtTokenOptions
{
    public const string KeyName = "JwtTokenOptions";
    [Required(ErrorMessage = "The {0} is required.")]
    public Secret Secret { get; init; } = null!;
    [Required(ErrorMessage = "The {0} is required.")]
    [Range(1, 5, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int AccessTokenExpirationMinutes { get; init; }
    [Required(ErrorMessage = "The {0} is required.")]
    [Range(60, 1440, ErrorMessage = "Value for {0} must be greater than {1}")]
    public int RefreshTokenExpirationMinutes { get; init; }
    [Required(ErrorMessage = "The {0} is required.")]
    public string Issuer { get; init; } = null!;
    [Required(ErrorMessage = "The {0} is required.")]
    public string Audience { get; init; } = null!;
    //public IReadOnlyCollection<string> Roles { get; init; }
}
