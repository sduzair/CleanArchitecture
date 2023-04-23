using Domain.Products.Entities;
using Domain.Products.ValueObjects;

namespace Application.Products;
public interface IProductsService
{
    Task<(int, Guid)> CreateProduct(string name, string description, decimal unitPrice);
    Task<int> DeleteProduct(ProductId id);
    Task<Product?> GetProduct(ProductId id);
    Task<IEnumerable<Product>> GetProducts();
    Task<int> UpdateProduct(ProductId id, string name, string description, decimal unitPrice);
}
