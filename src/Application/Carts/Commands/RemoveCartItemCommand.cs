using Application;
using Application.Common.Security;
using Application.Common.Security.Policies;

using Domain.Carts.Entities;
using Domain.Carts.ValueObjects;

using FluentResults;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Presentation.Carts;

/// <summary>
/// The quantity in of the <see cref="CartItem"/> in the <see cref="Cart"/> is incremented by one.
/// </summary>
/// <param name="CartId"></param>
/// <param name="CartItem">The quantity property of the <see cref="CartItem"/> is ignored.</param>
[ApplicationAuthorize(Policy = nameof(CartPolicy))]
public record RemoveCartItemCommand(CartId CartId, CartItem CartItem) : IRequest<Result>;

internal sealed class RemoveCartItemCommandHandler : IRequestHandler<RemoveCartItemCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public RemoveCartItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
    {
        var cart = await _context.Carts
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == request.CartId, cancellationToken);

        if (cart is null)
        {
            return Result.Fail("Cart not found");
        }

        cart.RemoveItem(request.CartItem);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}