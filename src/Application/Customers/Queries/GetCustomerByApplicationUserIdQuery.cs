using Application.Customers.Errors;

using Domain.Customers;

using FluentResults;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Persistence;

namespace Application.Customers.Queries;

public record GetCustomerByApplicationUserIdQuery(Guid ApplicationUserId) : IRequest<Result<Customer>>
{
    internal class Handler : IRequestHandler<GetCustomerByApplicationUserIdQuery, Result<Customer>>
    {
        private readonly AppDbContext _context;

        public Handler(AppDbContext context)
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

