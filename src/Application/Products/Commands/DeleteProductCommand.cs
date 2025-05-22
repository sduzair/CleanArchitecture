using Application.Products.Errors;

using Domain.Products.ValueObjects;

using FluentResults;

using MediatR;

using Persistence;

namespace Application.Products.Commands;

public record DeleteProductCommand(ProductId Id) : IRequest<Result>
{
    public sealed class Handler : IRequestHandler<DeleteProductCommand, Result>
    {
        private readonly AppDbContext _applicationDbContext;

        public Handler(AppDbContext applicationDbContext)
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
}
