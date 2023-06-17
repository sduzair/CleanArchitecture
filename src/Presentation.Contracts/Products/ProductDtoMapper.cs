using Application.Products.Commands;

using Domain.Products;
using Domain.Products.ValueObjects;

using FluentResults;

using MediatR;

namespace Presentation.Contracts.Products;

public static class ProductDtoMapper
{
    public static ProductDto MapTo(this Product product)
    {
        return new ProductDto(product.Id.Value, product.Name, product.Description, product.Price);
    }
    public static IRequest<Result> MapToUpdateProductCommand(this ProductDto product)
    {
        return new UpdateProductCommand(
            ProductId.From(product.Id),
            product.Name,
            product.Description,
            product.UnitPrice);
    }
}
