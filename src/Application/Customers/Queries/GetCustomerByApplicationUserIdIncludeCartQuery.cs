using Application.Common.Security;
using Application.Common.Security.Policies;

using Domain.Customers;

using FluentResults;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Customers.Queries;

/// <summary>
/// This query is used to get a customer by their application user id.
/// </summary>
/// <param name="ApplicaitonUserId">This the foreign key to the Users table</param>
[ApplicationAuthorize(Policy = nameof(CustomerPolicy))]
public record GetCustomerByApplicationUserIdIncludeCartQuery(Guid ApplicaitonUserId) : IRequest<Result<Customer>>;

internal sealed class GetCustomerByApplicationUserIdIncludeCartQueryHandler : IRequestHandler<GetCustomerByApplicationUserIdIncludeCartQuery, Result<Customer>>
{
    private readonly IApplicationDbContext _context;

    public GetCustomerByApplicationUserIdIncludeCartQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Customer>> Handle(GetCustomerByApplicationUserIdIncludeCartQuery request, CancellationToken cancellationToken)
    {
        var customer = await _context.Customers.Include(c => c.Cart)
            .SingleOrDefaultAsync(customer => customer.ApplicationUserId == request.ApplicaitonUserId, cancellationToken);

        if (customer is null)
        {
            return Result.Fail("Customer not found");
        }

        return Result.Ok(customer);
    }
}
