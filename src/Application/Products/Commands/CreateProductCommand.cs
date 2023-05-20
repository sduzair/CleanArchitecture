using Application.Common.Security;
using Application.Common.Security.Policies;

using Domain.Products;

using FluentResults;

using FluentValidation;

using MediatR;

namespace Application.Products.Commands;

[ApplicationAuthorize(Policy = nameof(ProductsManagementPolicy))]
public record CreateProductCommand(string Name, string Description, decimal UnitPrice, int Stock) : IRequest<Result<Guid>>;

internal sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _applicationDbContext;

    public CreateProductCommandHandler(IApplicationDbContext context)
    {
        _applicationDbContext = context;
    }

    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = Product.Create(request.Name, request.Description, request.UnitPrice, request.Stock);
        _applicationDbContext.Products.Add(product);
        _ = await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return product.Id!.Value;
    }
}

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(v => v.Name).NotEmpty().MaximumLength(200);
        RuleFor(v => v.Description).NotEmpty().MaximumLength(2000);
        RuleFor(v => v.UnitPrice).NotEmpty().GreaterThan(0);
    }
}
