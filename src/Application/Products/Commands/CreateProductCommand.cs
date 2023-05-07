using Application.Common.Security;

using Domain.Products.Entities;

using FluentResults;

using MediatR;

namespace Application.Products.Commands;

[ApplicationAuthorize(Policy = ProductManagementPolicy.PolicyName)]
public record CreateProductCommand(string Name, string Description, decimal UnitPrice) : IRequest<Result<Guid>>;

internal sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _applicationDbContext;

    public CreateProductCommandHandler(IApplicationDbContext context)
    {
        _applicationDbContext = context;
    }

    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = Product.Create(request.Name, request.Description, request.UnitPrice);
        _applicationDbContext.Products.Add(product);
        int entries = await _applicationDbContext.SaveChangesAsync(cancellationToken);

        //add entries changed to meta data

        return product.Id!.Value;
    }
}
