using Domain.Carts;
using Domain.Carts.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Carts;

/// <summary>
/// Not configured as an entity as it is a value object of <see cref="Cart"/> aggregate. Not configured as ownsmany in Cart config as Id autoincrement not supported in composite key with CartId.
/// </summary>
internal sealed class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> b)
    {
        b.Property<int>("Id")
            .ValueGeneratedOnAdd();

        b.HasKey("Id");

        b.HasOne<Cart>()
            .WithMany(cart => cart.Items)
            .HasForeignKey("CartId")
            .IsRequired();

        b.HasOne(item => item.Product)
            .WithMany()
            .HasForeignKey(item => item.ProductId)
            .IsRequired(false);
    }
}
