using Application.Common.Security;
using Application.Common.Security.Policies;

using Domain.Carts;
using Domain.Carts.ValueObjects;
using Domain.Customers.ValueObjects;

using FluentResults;

using MediatR;

namespace Application.Carts.Commands;

public record CreateCartCommand(CustomerId CustomerId) : IRequest<Result<CartId>>
{
    internal class Handler : IRequestHandler<CreateCartCommand, Result<CartId>>
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<CartId>> Handle(CreateCartCommand request, CancellationToken cancellationToken)
        {
            var cart = Cart.Create(request.CustomerId);
            await _context.Carts.AddAsync(cart, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Ok(cart.Id);
        }
    }
}
