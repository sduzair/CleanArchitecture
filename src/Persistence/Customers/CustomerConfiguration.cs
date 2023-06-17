using Domain.Carts;
using Domain.Carts.ValueObjects;
using Domain.Customers;
using Domain.Customers.ValueObjects;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Persistence.Identity;

namespace Persistence.Customers;

internal class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        PropertiesConfig(builder);

        ApplicationUserEntityRelationshipConfig(builder);
    }

    private static void ApplicationUserEntityRelationshipConfig(EntityTypeBuilder<Customer> builder)
    {
        ///no navigation property to <see cref="User"/> as the class is not part of the domain layer
        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<Customer>(e => e.ApplicationUserId);
    }

    private static void PropertiesConfig(EntityTypeBuilder<Customer> builder)
    {
        builder.Property(e => e.Id)
            .HasConversion(x => x!.Value, x => CustomerId.From(x));
    }
}
