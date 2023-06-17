using Domain.Products;

using FluentResults;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Persistence;

namespace Application.Products.Queries;

public record class GetProductsQuery : IRequest<Result<IEnumerable<Product>>>
{
    internal sealed class Handler : IRequestHandler<GetProductsQuery, Result<IEnumerable<Product>>>

    {
        private readonly AppDbContext _applicationDbContext;
        public Handler(AppDbContext context)
        {
            _applicationDbContext = context;
        }
        public async Task<Result<IEnumerable<Product>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            List<Product> products = await _applicationDbContext.Products.AsNoTracking().ToListAsync(cancellationToken);
            return products;
        }
    }
}

