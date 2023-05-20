using Application;
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
public record AddItemOrUpdateQuantityCommand(CartId CartId, CartItem CartItem) : IRequest<Result>;

internal sealed class AddItemOrUpdateQuantityCommandHandler : IRequestHandler<AddItemOrUpdateQuantityCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public AddItemOrUpdateQuantityCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(AddItemOrUpdateQuantityCommand request, CancellationToken cancellationToken)
    {
        var (cartId, cartItem) = request;

        //TODO - check whether specifying include is necessary because Items is "Owned" by Cart
        var cart = await _context.Carts
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == cartId, cancellationToken);

        if (cart is null)
        {
            return Result.Fail(new CartNotFoundError(cartId));
        }
        cart.UpdateItemQuantityOrAddItem(request.CartItem);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}
