using Application.Common.Security;

using Domain.Products.Entities;
using Domain.Products.Errors;
using Domain.Products.ValueObjects;

using FluentResults;

using MediatR;

namespace Application.Products.Queries;

[ApplicationAuthorize(Policy = ProductViewPolicy.PolicyName)]
public record GetProductByIdQuery(ProductId Id) : IRequest<Result<Product>>;

internal sealed class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<Product>>
{
    private readonly IApplicationDbContext _applicationDbContext;
    public GetProductByIdQueryHandler(IApplicationDbContext context)
    {
        _applicationDbContext = context;
    }
    public async Task<Result<Product>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _applicationDbContext.Products.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
        if (product is null)
        {
            return Result.Fail(new ProductNotFoundError(request.Id));
        }
        return product;
    }
}
