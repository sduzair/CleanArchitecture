using Application.Common.Security;
using Application.Common.Security.Policies;

using Domain.Carts.Entities;
using Domain.Carts.Errors;
using Domain.Carts.ValueObjects;

using FluentResults;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Carts.Commands;

/// <summary>
/// Add item or update quantity of item in cart if item already exits.  
/// </summary>
/// <param name="CartId"></param>
/// <param name="CartItem"></param>
[ApplicationAuthorize(Policy = nameof(CartPolicy))]
public record AddItemOrUpdateQuantityCommand(CartId CartId, CartItem CartItem) : IRequest<Result>
{
    internal sealed class Handler : IRequestHandler<AddItemOrUpdateQuantityCommand, Result>
    {
        private readonly IApplicationDbContext _context;

        public Handler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(AddItemOrUpdateQuantityCommand request, CancellationToken cancellationToken)
        {
            var (cartId, cartItem) = request;

            var cart = await _context.Carts
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == cartId, cancellationToken);

            if (cart is null)
            {
                return Result.Fail(new CartNotFoundError(cartId));
            }

            var result = cart.AddItemOrUpdateQuantity(request.CartItem);

            if (result.IsFailed)
            {
                return result;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}

