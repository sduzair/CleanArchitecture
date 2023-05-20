using Application;
using Application.Common.Security;
using Application.Common.Security.Policies;

using Domain.Customers;
using Domain.Customers.ValueObjects;

using FluentResults;

using FluentValidation;

using MediatR;

namespace Application.Customers.Commands;

[ApplicationAuthorize(Policy = nameof(CustomerPolicy))]
public record CreateCustomerCommand(Guid ApplicationUserId) : IRequest<Result<CustomerId>>;

internal class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<CustomerId>>
{
    private readonly IApplicationDbContext _context;
    public CreateCustomerCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result<CustomerId>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = Customer.Create(request.ApplicationUserId);
        await _context.Customers.AddAsync(customer, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return customer.Id;
    }
}

internal class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.ApplicationUserId).NotEmpty();
    }
}