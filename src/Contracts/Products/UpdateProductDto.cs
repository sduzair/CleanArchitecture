using Domain.Products.ValueObjects;

namespace Presentation.Contracts.Products;

public record UpdateProductDto(ProductId Id, string Name, string Description, decimal UnitPrice);