using Domain.Products.Entities;
using Domain.Products.ValueObjects;

using Microsoft.EntityFrameworkCore;

namespace Application.Products;
internal sealed class ProductsService : IProductsService
{
    private readonly IApplicationDbContext _applicationDbContext;

    public ProductsService(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }
    public async Task<IEnumerable<Product>> GetProducts()
    {
        return await _applicationDbContext.Products.ToListAsync();
    }
    public async Task<(int, Guid)> CreateProduct(string name, string description, decimal unitPrice)
    {
        var product = Product.Create(name, description, unitPrice);
        _applicationDbContext.Products.Add(product);
        return (await _applicationDbContext.SaveChangesAsync(), product.Id!.IdValue);
    }

    public async Task<int> DeleteProduct(ProductId id)
    {
        var product = await _applicationDbContext.Products.FindAsync(id);
        if (product is null)
        {
            return 0;
        }
        _applicationDbContext.Products.Remove(product);
        return await _applicationDbContext.SaveChangesAsync();
    }

    public async Task<Product?> GetProduct(ProductId id)
    {
        return await _applicationDbContext.Products.FindAsync(id);
    }

    public async Task<int> UpdateProduct(ProductId id, string name, string description, decimal unitPrice)
    {
        var product = await _applicationDbContext.Products.FindAsync(id);
        if (product is null)
        {
            return 0;
        }
        product.Update(name, description, unitPrice);
        _applicationDbContext.Products.Update(product);
        return await _applicationDbContext.SaveChangesAsync();
    }
}
