using Application.Common.Security;

using Domain.Products.Entities;

using FluentResults;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Products.Queries;

[ApplicationAuthorize(Policy = ProductViewPolicy.PolicyName)]
public record class GetProductsQuery : IRequest<Result<IEnumerable<Product>>>;

internal sealed class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<IEnumerable<Product>>>

{
    private readonly IApplicationDbContext _applicationDbContext;
    public GetProductsQueryHandler(IApplicationDbContext context)
    {
        _applicationDbContext = context;
    }
    public async Task<Result<IEnumerable<Product>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        List<Product> products = await _applicationDbContext.Products.ToListAsync(cancellationToken);
        return products;
    }
}
