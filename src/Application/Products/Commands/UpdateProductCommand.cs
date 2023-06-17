using Application.Common.Security;
using Application.Common.Security.Policies;

using Domain.Products.Errors;
using Domain.Products.ValueObjects;

using FluentResults;

using MediatR;

using Persistence;

namespace Application.Products.Commands;

public record UpdateProductCommand(ProductId Id, string Name, string Description, decimal UnitPrice) : IRequest<Result>
{
    internal sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result>
    {
        private readonly AppDbContext _applicationDbContext;
        public UpdateProductCommandHandler(AppDbContext context)
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

            return Result.Ok();
        }
    }
}
