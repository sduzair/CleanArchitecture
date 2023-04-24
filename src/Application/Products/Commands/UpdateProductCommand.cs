using Domain.Products.ValueObjects;

using FluentResults;

using MediatR;

namespace Application.Products.Commands;
public record UpdateProductCommand(ProductId Id, string Name, string Description, decimal UnitPrice) : IRequest<Result>;

internal class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result>
{
    private readonly IApplicationDbContext _applicationDbContext;
    public UpdateProductCommandHandler(IApplicationDbContext context)
    {
        _applicationDbContext = context;
    }
    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _applicationDbContext.Products.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
        if (product is null)
        {
            return Result.Fail(new Errors.ProductNotFoundError(request.Id));
        }
        product.Update(request.Name, request.Description, request.UnitPrice);
        int entries = await _applicationDbContext.SaveChangesAsync(cancellationToken);

        //add entries changed to meta data

        return Result.Ok();
    }
}
