using Domain.Carts;
using Domain.Customers;
using Domain.Products;

using Microsoft.EntityFrameworkCore;

namespace Application;
public interface IApplicationDbContext
{
    DbSet<Product> Products { get; set; }
    DbSet<Cart> Carts { get; set; }
    DbSet<Customer> Customers { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
