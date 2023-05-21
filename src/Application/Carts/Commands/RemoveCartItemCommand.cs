using Application.Common.Security;
using Application.Common.Security.Policies;

using Domain.Carts.Entities;
using Domain.Carts.Errors;
using Domain.Carts.ValueObjects;

using FluentResults;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Carts.Commands;

[ApplicationAuthorize(Policy = nameof(CartPolicy))]
public record RemoveCartItemCommand(CartId CartId, CartItem CartItem) : IRequest<Result>
{
    internal sealed class Handler : IRequestHandler<RemoveCartItemCommand, Result>
    {
        private readonly IApplicationDbContext _context;

        public Handler(IApplicationDbContext context)
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

            cart.RemoveItem(cartItem);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}
