using System.Runtime.Serialization;
using System.Security.Claims;

using Application.Common.Interfaces;

using Microsoft.AspNetCore.Http;

using Presentation.Exceptions;

namespace Presentation.Services;
internal sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _contextAccessor;

    public CurrentUserService(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public string UserId => _contextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new ClaimsPrincipalNullException();

    public ClaimsPrincipal User => _contextAccessor.HttpContext!.User;

}
