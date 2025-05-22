using Domain.Products;
using Domain.Products.ValueObjects;

using FluentResults;

using MediatR;

using Persistence;

namespace Application.Products.Commands;

public record CreateProductCommand(string Name, string Description, decimal UnitPrice, int Stock) : IRequest<Result<Guid>>
{
    public sealed class Handler : IRequestHandler<CreateProductCommand, Result<Guid>>
    {
        private readonly AppDbContext _applicationDbContext;

        public Handler(AppDbContext context)
        {
            _applicationDbContext = context;
        }

        public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var result = Product.Create(ProductId.Create(), request.Name, request.Description, request.UnitPrice, request.Stock);

            _applicationDbContext.Products.Add(result.Value);

            _ = await _applicationDbContext.SaveChangesAsync(cancellationToken);

            return result.Value.Id!.Value;
        }
    }
}
