using System.Net;

using Domain.Carts.Entities;
using Domain.Common.ErrorTypes;

using FluentResults;

namespace Application.Carts.Errors;
internal class CartItemNotFoundError : Error, IProblemDetailsError
{
    public CartItemNotFoundError(CartItem cartItem)
    {
        StatusCode = (int)HttpStatusCode.NotFound;
        Title = nameof(CartItemNotFoundError);
        Message = $"Cart item with following properties not found: {cartItem}";
    }

    public string Title { get; init; }
    public int StatusCode { get; init; }
}