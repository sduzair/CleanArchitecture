using Application;

using Domain.Customers.Errors;

using FluentResults;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Presentation.Customers;
public record DeleteCustomerCommand(Guid ApplicationUserId) : IRequest<Result>;

internal class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteCustomerCommandHandler(IApplicationDbContext context)
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