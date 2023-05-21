using Application.Common.Security;
using Application.Common.Security.Policies;

using Domain.Products;

using FluentResults;

using MediatR;

namespace Application.Products.Commands;

[ApplicationAuthorize(Policy = nameof(ProductsManagementPolicy))]
public record CreateProductCommand(string Name, string Description, decimal UnitPrice, int Stock) : IRequest<Result<Guid>>
{
    internal sealed class Handler : IRequestHandler<CreateProductCommand, Result<Guid>>
    {
        private readonly IApplicationDbContext _applicationDbContext;

        public Handler(IApplicationDbContext context)
        {
            _applicationDbContext = context;
        }

        public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var result = Product.Create(request.Name, request.Description, request.UnitPrice, request.Stock);

            _applicationDbContext.Products.Add(result.Value);

            _ = await _applicationDbContext.SaveChangesAsync(cancellationToken);

            return result.Value.Id!.Value;
        }
    }
}
