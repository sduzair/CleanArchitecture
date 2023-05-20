using Application.Common.Security;
using Application.Common.Security.Policies;

using Domain.Products.Errors;
using Domain.Products.ValueObjects;

using FluentResults;

using FluentValidation;

using MediatR;

namespace Application.Products.Commands;

[ApplicationAuthorize(Policy = nameof(ProductsManagementPolicy))]
public record UpdateProductCommand(ProductId Id, string Name, string Description, decimal UnitPrice) : IRequest<Result>;

internal sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result>
{
    private readonly IApplicationDbContext _applicationDbContext;
    public UpdateProductCommandHandler(IApplicationDbContext context)
    {
        _applicationDbContext = context;
    }
    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _applicationDbContext.Products.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
        if (product is null)
        {
            return Result.Fail(new ProductNotFoundError(request.Id));
        }
        product.Update(request.Name, request.Description, request.UnitPrice);
        _ = await _applicationDbContext.SaveChangesAsync(cancellationToken);

        //TODO - add entries changed to meta data

        return Result.Ok();
    }
}

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(v => v.Id.Value).NotEmpty();
        RuleFor(v => v.Name).NotEmpty().MaximumLength(200);
        RuleFor(v => v.Description).NotEmpty().MaximumLength(2000);
        RuleFor(v => v.UnitPrice).NotEmpty().GreaterThan(0);
    }
}
