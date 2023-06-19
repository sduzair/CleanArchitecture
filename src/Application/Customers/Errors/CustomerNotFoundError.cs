using System.Net;

using Domain.Common.ErrorTypes;

using FluentResults;

namespace Application.Customers.Errors;
public sealed class CustomerNotFoundError : Error, IProblemDetailsError
{
    public CustomerNotFoundError(Guid applicationUserId)
    {
        StatusCode = (int)HttpStatusCode.NotFound;
        Title = nameof(CustomerNotFoundError);
        Message = $"Customer with application user id {applicationUserId} not found";
    }

    public int StatusCode { get; init; }
    public string Title { get; init; }
}
