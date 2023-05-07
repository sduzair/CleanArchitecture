using System.Reflection;

using Application.Common.Security;
using Application.Products;

using Infrastructure.Utilities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Identity.EFConfigurations;
internal class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        // Each ApplicationRole can have many entries in the UserRole join table
        builder.HasMany(e => e.ApplicatoinUserRoles)
            .WithOne(e => e.ApplicationRole)
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();

        Type productRolesClassType = typeof(ProductsRoles);
        var productRoles = productRolesClassType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
            .Select(x => x.GetRawConstantValue()!.ToString())
            .ToArray();

        builder.HasData(productRoles.Select(x => new ApplicationRole { Id = GuidHelper.GenerateDeterministicGuid(x!), Name = x, NormalizedName = x!.ToUpper() }));

        Type visitorRolesClassType = typeof(VisitorRoles);
        var visitorRoles = visitorRolesClassType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
            .Select(x => x.GetRawConstantValue()!.ToString())
            .ToArray();

        builder.HasData(visitorRoles.Select(x => new ApplicationRole { Id = GuidHelper.GenerateDeterministicGuid(x!), Name = x, NormalizedName = x!.ToUpper() }));
    }
}
