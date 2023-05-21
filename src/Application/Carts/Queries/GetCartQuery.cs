using Application.Common.Security;
using Application.Common.Security.Policies;

using Domain.Carts;
using Domain.Carts.Errors;
using Domain.Carts.ValueObjects;

using FluentResults;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Carts.Queries;

[ApplicationAuthorize(Policy = nameof(CartPolicy))]
public record GetCartQuery(CartId CartId) : IRequest<Result<Cart>>
{
    internal sealed class Handler : IRequestHandler<GetCartQuery, Result<Cart>>
    {
        private readonly IApplicationDbContext _context;

        public Handler(IApplicationDbContext context)
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
