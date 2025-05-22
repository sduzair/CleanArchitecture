using Application.Common.Security;
using Application.Common.Security.Policies;

using Domain.Customers;
using Domain.Customers.ValueObjects;

using FluentResults;

using MediatR;

using Persistence;

namespace Application.Customers.Commands;

public record CreateCustomerCommand(Guid UserId) : IRequest<Result<CustomerId>>
{
    public class Handler : IRequestHandler<CreateCustomerCommand, Result<CustomerId>>
    {
        private readonly AppDbContext _context;
        public Handler(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Result<CustomerId>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = Customer.Create(request.UserId);
            await _context.Customers.AddAsync(customer, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return customer.Id;
        }
    }
}
