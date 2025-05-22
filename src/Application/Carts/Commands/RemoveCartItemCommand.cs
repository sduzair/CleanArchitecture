using Application.Carts.Errors;

using Domain.Carts.Entities;
using Domain.Carts.ValueObjects;

using FluentResults;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Persistence;

namespace Application.Carts.Commands;

public record RemoveCartItemCommand(CartId CartId, CartItem CartItem) : IRequest<Result>
{
    internal sealed class Handler : IRequestHandler<RemoveCartItemCommand, Result>
    {
        private readonly AppDbContext _context;

        public Handler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
        {
            var (cartId, cartItem) = request;
            var cart = await _context.Carts
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == cartId, cancellationToken);

            if (cart is null)
            {
                return Result.Fail(new CartNotFoundError(cartId));
            }

            var found = cart.RemoveItem(cartItem);

            if (!found)
            {
                return Result.Fail(new CartItemNotFoundError(cartItem));
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}
