using Application.Carts.Errors;

using Domain.Carts;
using Domain.Carts.ValueObjects;

using FluentResults;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Persistence;

namespace Application.Carts.Queries;

public record GetCartQuery(CartId CartId) : IRequest<Result<Cart>>
{
    internal sealed class Handler : IRequestHandler<GetCartQuery, Result<Cart>>
    {
        private readonly AppDbContext _context;

        public Handler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Cart>> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            var cart = await _context.Carts
                .Include(cart => cart.Items)
                .SingleOrDefaultAsync(cart => cart.Id == request.CartId, cancellationToken);

            if (cart is null)
            {
                return Result.Fail(new CartNotFoundError(request.CartId));
            }

            return cart;
        }
    }
}
