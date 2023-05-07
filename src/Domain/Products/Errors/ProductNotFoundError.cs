using System.Net;

using Domain.Common.ErrorTypes;
using Domain.Products.ValueObjects;

using FluentResults;

namespace Domain.Products.Errors;
public sealed class ProductNotFoundError : Error, IProblemDetailsError
{
    public ProductNotFoundError(ProductId id)
    {
        StatusCode = (int)HttpStatusCode.NotFound;
        Title = nameof(ProductNotFoundError);
        Message = $"Product with id {id.Value} not found";
    }

    public int StatusCode { get; init; }
    public string Title { get; init; }
}
