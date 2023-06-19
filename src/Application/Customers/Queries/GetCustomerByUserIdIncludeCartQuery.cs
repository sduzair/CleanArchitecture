using Application.Customers.Errors;

using Domain.Customers;

using FluentResults;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Persistence;

namespace Application.Customers.Queries;

/// <summary>
/// This query is used to get a customer by their application user id.
/// </summary>
/// <param name="ApplicaitonUserId">This the foreign key to the Users table</param>
public record GetCustomerByUserIdIncludeCartQuery(Guid ApplicationUserId) : IRequest<Result<Customer>>
{
    internal sealed class Handler : IRequestHandler<GetCustomerByUserIdIncludeCartQuery, Result<Customer>>
    {
        private readonly AppDbContext _context;

        public Handler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Customer>> Handle(GetCustomerByUserIdIncludeCartQuery request, CancellationToken cancellationToken)
        {
            var customer = await _context.Customers.Include(c => c.Cart)
                .SingleOrDefaultAsync(customer => customer.ApplicationUserId == request.ApplicationUserId, cancellationToken);

            if (customer is null)
            {
                return Result.Fail(new CustomerNotFoundError(request.ApplicationUserId));
            }

            return Result.Ok(customer);
        }
    }
}
