using Infrastructure.Identity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Identity.EFConfigurations;
internal class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.HasData(
        new ApplicationRole
        {
            Id = Guid.NewGuid(),
            Name = "Admin",
            NormalizedName = "ADMIN"
        },
        new ApplicationRole
        {
            Id = Guid.NewGuid(),
            Name = "Visitor",
            NormalizedName = "VISITOR"
        }
        );
    }
}
