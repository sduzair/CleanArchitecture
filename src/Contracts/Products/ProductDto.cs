namespace Presentation.Contracts.Products;

public record ProductDto(Guid Id, string Name, string Description, decimal UnitPrice);
