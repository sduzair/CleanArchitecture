using Application.Products.Errors;

using Domain.Products;
using Domain.Products.ValueObjects;

using FluentResults;

using MediatR;

using Persistence;

namespace Application.Products.Queries;

public record GetProductByIdQuery(ProductId Id) : IRequest<Result<Product>>
{
    internal sealed class Handler : IRequestHandler<GetProductByIdQuery, Result<Product>>
    {
        private readonly AppDbContext _applicationDbContext;
        public Handler(AppDbContext context)
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
}
