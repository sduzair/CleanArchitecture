using Application.Common.Security;
using Application.Common.Security.Policies;

using Domain.Carts;
using Domain.Carts.Entities;
using Domain.Carts.Errors;
using Domain.Carts.ValueObjects;

using FluentResults;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Carts.Commands;

/// <summary>
/// Queries the <see cref="Cart.CustomerId"/> field in the <see cref="Cart"/> table to see if a cart exists for the customer with the <paramref name="CustomerId"/>. If not, a new cart is created with the <paramref name="CartItems"/>. If a cart exists, the <paramref name="CartItems"/> are added to the existing cart.
/// </summary>
/// <param name="CustomerId"></param>
/// <param name="CartItems"></param>
public record AddItemsToCartCommand(CartId CustomerId, IReadOnlyCollection<CartItem> CartItems) : IRequest<Result>
{
    internal class Handler : IRequestHandler<AddItemsToCartCommand, Result>
    {
        private readonly IApplicationDbContext _context;

        public Handler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(AddItemsToCartCommand request, CancellationToken cancellationToken)
        {
            var (cartId, cartItems) = request;

            var cart = await _context.Carts
                .Include(cart => cart.Items)
                .SingleOrDefaultAsync(cart => cart.Id == cartId, cancellationToken);

            Result? result;
            if (cart is null)
            {
                return Result.Fail(new CartNotFoundError(cartId));
            }
            else
            {
                result = cart.AddItems(cartItems);
            }

            if (result.IsFailed)
            {
                return result;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}

