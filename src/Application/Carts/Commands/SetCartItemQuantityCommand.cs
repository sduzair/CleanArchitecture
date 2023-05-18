using Application;
using Application.Common.Security;
using Application.Common.Security.Policies;

using Domain.Carts.Entities;
using Domain.Carts.ValueObjects;

using FluentResults;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Presentation.Carts;

[ApplicationAuthorize(Policy = nameof(CartPolicy))]
public record SetCartItemQuantityCommand(CartId CartId, CartItem CartItem) : IRequest<Result>;

internal sealed class AddCartItemCommandHandler : IRequestHandler<SetCartItemQuantityCommand, Result>
{
    private readonly IApplicationDbContext _context;
    public AddCartItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result> Handle(SetCartItemQuantityCommand request, CancellationToken cancellationToken)
    {
        var cart = await _context.Carts
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == request.CartId, cancellationToken);
        if (cart is null)
        {
            return Result.Fail("Cart not found");
        }
        cart.AddItem(request.CartItem);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}
