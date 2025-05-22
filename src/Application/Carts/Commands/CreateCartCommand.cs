using Domain.Carts;
using Domain.Carts.ValueObjects;
using Domain.Customers.ValueObjects;

using FluentResults;

using MediatR;

using Persistence;

namespace Application.Carts.Commands;

public record CreateCartCommand(CustomerId CustomerId) : IRequest<Result<CartId>>
{
    public sealed class Handler : IRequestHandler<CreateCartCommand, Result<CartId>>
    {
        private readonly AppDbContext _context;
        public Handler(AppDbContext context)
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
