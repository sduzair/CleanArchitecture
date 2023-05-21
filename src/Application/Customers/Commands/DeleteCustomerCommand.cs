using Application;
using Application.Common.Security;
using Application.Common.Security.Policies;

using Domain.Customers.Errors;

using FluentResults;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Customers.Commands;

//[ApplicationAuthorize(Policy = nameof(CustomerPolicy))]
public record DeleteCustomerCommand(Guid ApplicationUserId) : IRequest<Result>
{
    internal class Handler : IRequestHandler<DeleteCustomerCommand, Result>
    {
        private readonly IApplicationDbContext _context;

        public Handler(IApplicationDbContext context)
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