using Application.Customers.Errors;

using FluentResults;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Persistence;

namespace Application.Customers.Commands;

public record DeleteCustomerCommand(Guid ApplicationUserId) : IRequest<Result>
{
    internal sealed class Handler : IRequestHandler<DeleteCustomerCommand, Result>
    {
        private readonly AppDbContext _context;

        public Handler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _context.Customers.SingleOrDefaultAsync(c => c.ApplicationUserId == request.ApplicationUserId, cancellationToken);
            if (customer == null)
            {
                return Result.Fail(new CustomerNotFoundError(request.ApplicationUserId));
            }
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
    }
}