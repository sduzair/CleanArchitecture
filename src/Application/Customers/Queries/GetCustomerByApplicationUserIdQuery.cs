using Application.Common.Security;
using Application.Common.Security.Policies;

using Domain.Customers;
using Domain.Customers.Errors;

using FluentResults;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Customers.Queries;

[ApplicationAuthorize(Policy = nameof(CustomerPolicy))]
public record GetCustomerByApplicationUserIdQuery(Guid ApplicationUserId) : IRequest<Result<Customer>>
{
    internal class Handler : IRequestHandler<GetCustomerByApplicationUserIdQuery, Result<Customer>>
    {
        private readonly IApplicationDbContext _context;

        public Handler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Customer>> Handle(GetCustomerByApplicationUserIdQuery request, CancellationToken cancellationToken)
        {
            var customer = await _context.Customers.SingleOrDefaultAsync(customer => customer.ApplicationUserId == request.ApplicationUserId, cancellationToken: cancellationToken);

            if (customer == null)
            {
                return Result.Fail(new CustomerNotFoundError(request.ApplicationUserId));
            }

            return Result.Ok(customer);
        }
    }
}

