using Application.Products.Commands;

using Domain.Products;
using Domain.Products.ValueObjects;

using FluentResults;

using MediatR;

using Presentation.Carts;

namespace Presentation.Products;

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
    public static CartItemDto MapToCartItemDto(this Product product, int quantity)
    {
        return new CartItemDto(product.Id.Value, product.Name, product.Description, product.Price, quantity);
    }
}
