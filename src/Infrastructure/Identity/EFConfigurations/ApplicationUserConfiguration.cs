using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Identity.EFConfigurations;
internal class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        // Each ApplicationUser can have many entries in the UserRole join table
        builder.HasMany(e => e.ApplicationUserRoles)
            .WithOne(e => e.ApplicationUser)
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();

        //builder.Metadata.FindNavigation(nameof(ApplicationUser.ApplicatoinUserRoles))!
            //.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
