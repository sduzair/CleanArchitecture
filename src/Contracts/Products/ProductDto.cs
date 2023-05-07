using Application.Products.Commands;

using Domain.Products.Entities;
using Domain.Products.ValueObjects;

using FluentResults;

using MediatR;

namespace Application.Products.Queries;

public record ProductDto(Guid Id, string Name, string Description, decimal UnitPrice)
{
    public static ProductDto MapFrom(Product value)
    {
        return new ProductDto(value.Id!.Value, value.Name, value.Description, value.Price);
    }

    public IRequest<Result> MapToUpdateProductCommand()
    {
        return new UpdateProductCommand(
            ProductId.Create(Id),
            Name,
            Description,
            UnitPrice);
    }
}
