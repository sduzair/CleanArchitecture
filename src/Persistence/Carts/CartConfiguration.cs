using Domain.Carts;
using Domain.Carts.Entities;
using Domain.Carts.ValueObjects;
using Domain.Customers;
using Domain.Customers.ValueObjects;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Carts;

internal class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> b)
    {
        PropertiesConfig(b);

        CustomerEntityConfig(b);
        CartItemLocalEntityConfig(b);
    }

    private static void CartItemLocalEntityConfig(EntityTypeBuilder<Cart> b)
    {
        //to use the backing field for access
        b.Navigation<CartItem>(cart => cart.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }

    private static void CustomerEntityConfig(EntityTypeBuilder<Cart> b)
    {
        ///A <see cref="Cart"/> does not have a <see cref="Customer"/> when role is <see cref="VisitorRoles.Visitor"/> so IsRequired is set to false
        b.HasOne<Customer>()
            .WithOne(customer => customer.Cart)
            .HasForeignKey<Cart>(cart => cart.CustomerId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void PropertiesConfig(EntityTypeBuilder<Cart> b)
    {
        b.Property(c => c.Id)
            .ValueGeneratedNever()
            .HasConversion(e => e!.Value, v => CartId.From(v));

        b.Property(c => c.CustomerId)
            .HasConversion(e => e!.Value, v => CustomerId.From(v));
    }
}
