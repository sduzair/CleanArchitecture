using System.Net;

using Domain.Carts.ValueObjects;
using Domain.Common.ErrorTypes;

using FluentResults;

namespace Domain.Carts.Errors;

public class CartNotFoundError : Error, IProblemDetailsError
{
    public CartNotFoundError(CartId cartId)
    {
        StatusCode = (int)HttpStatusCode.NotFound;
        Title = nameof(CartNotFoundError);
        Message = $"Cart with id {cartId.Value} not found";
    }

    public string Title { get; init; }
    public int StatusCode { get; init; }
}