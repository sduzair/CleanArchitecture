using FluentResults;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Products.Queries;
public record class GetProductsQuery : IRequest<Result<IEnumerable<ProductDto>>>;

internal class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<IEnumerable<ProductDto>>>

{
    private readonly IApplicationDbContext _applicationDbContext;
    public GetProductsQueryHandler(IApplicationDbContext context)
    {
        _applicationDbContext = context;
    }
    public async Task<Result<IEnumerable<ProductDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _applicationDbContext.Products.ToListAsync(cancellationToken);
        return Result.Ok(products.Select(product => new ProductDto(product.Id!.Value, product.Name, product.Description, product.Price)));
    }
}
