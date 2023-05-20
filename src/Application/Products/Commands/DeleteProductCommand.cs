using Application.Common.Security;
using Application.Common.Security.Policies;

using Domain.Products.Errors;
using Domain.Products.ValueObjects;

using FluentResults;

using MediatR;

namespace Application.Products.Commands;

[ApplicationAuthorize(Policy = nameof(ProductsAdminPolicy))]
public record DeleteProductCommand(ProductId Id) : IRequest<Result>;

internal sealed class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result>
{
    private readonly IApplicationDbContext _applicationDbContext;

    public DeleteProductCommandHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _applicationDbContext.Products.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
        if (product is null)
        {
            return Result.Fail(new ProductNotFoundError(request.Id));
        }
        _applicationDbContext.Products.Remove(product);
        _ = await _applicationDbContext.SaveChangesAsync(cancellationToken);

        //add entries changed to meta data

        return Result.Ok();
    }
}
