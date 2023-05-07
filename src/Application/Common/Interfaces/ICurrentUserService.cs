using System.Security.Claims;

namespace Application.Common.Interfaces;

public interface ICurrentUserService
{
    string UserId { get; }
    ClaimsPrincipal User { get; }
}
