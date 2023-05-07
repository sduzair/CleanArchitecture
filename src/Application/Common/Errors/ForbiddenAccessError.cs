using System.Net;

using Domain.Common.ErrorTypes;

using FluentResults;

namespace Application.Common.Errors;
internal sealed class ForbiddenAccessError : Error, IProblemDetailsError
{
    public ForbiddenAccessError(string userId)
    {
        StatusCode = (int)HttpStatusCode.Forbidden;
        Title = nameof(ForbiddenAccessError);
        Message = $"User with id {userId} is not allowed to access this resource";
    }
    public int StatusCode { get; init; }
    public string Title { get; init; }
}
