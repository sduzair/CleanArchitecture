using Domain.Products.ValueObjects;

using FluentResults;

namespace Application.Products.Errors;
public sealed class ProductNotFoundError : Error
{
    public ProductNotFoundError(ProductId id) : base($"Product with id {id} not found") { }
}
