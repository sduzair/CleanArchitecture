using Domain.Products;
using Domain.Products.ValueObjects;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Products;
internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(x => x.Id)
            .HasConversion(x => x!.Value, x => new ProductId(x));

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100);
    }
}
