using Domain.Products.Entities;

using Microsoft.EntityFrameworkCore;

namespace Application;
public interface IApplicationDbContext
{
    DbSet<Product> Products { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    async Task<int> SaveChangesAsync()
    {
        return await SaveChangesAsync(default);
    }
}
