using Domain.Products.ValueObjects;

using FluentResults;

using MediatR;

namespace Application.Products.Queries;
public record GetProductByIdQuery(ProductId Id) : IRequest<Result<ProductDto>>;

internal class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IApplicationDbContext _applicationDbContext;
    public GetProductByIdQueryHandler(IApplicationDbContext context)
    {
        _applicationDbContext = context;
    }
    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _applicationDbContext.Products.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
        if (product is null)
        {
            return Result.Fail(new Errors.ProductNotFoundError(request.Id));
        }
        return new ProductDto(product.Id!.Value, product.Name, product.Description, product.Price);
    }
}
