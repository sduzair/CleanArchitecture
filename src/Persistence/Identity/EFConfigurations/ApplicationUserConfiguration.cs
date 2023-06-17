using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Persistence.Identity;

namespace Persistence.Identity.EFConfigurations;
internal class ApplicationUserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Each ApplicationUser can have many entries in the UserRole join table
        builder.HasMany(e => e.UserRoles)
            .WithOne(e => e.User)
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();

        //builder.Metadata.FindNavigation(nameof(ApplicationUser.ApplicatoinUserRoles))!
        //.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
