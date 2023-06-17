using System.Reflection;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Persistence.Utilities;

namespace Persistence.Identity.EFConfigurations;

internal sealed class ApplicationRoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        /// Each <see cref="Role"/> can have many entries in the <see cref="UserRole"/> join table
        builder.HasMany(e => e.UserRoles)
            .WithOne(e => e.Role)
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();

        var roles = GetRoleNames();

        builder.HasData(roles.Select(x => new Role { Id = GuidHelper.GenerateDeterministicGuid(x), Name = x, NormalizedName = x.ToUpper() }));
    }

    private static IReadOnlyList<string> GetRoleNames()
    {
        //Using Reflection to get all the roles
        var roleTypes = Assembly.GetAssembly(typeof(IRole))!.DefinedTypes
            .Where(t => t.IsAbstract && t.IsClass && t.GetInterfaces().Contains(typeof(IRole)));

        var roleTypeNames = roleTypes.Select(t => t.Name).ToList();

        return roleTypeNames.AsReadOnly();
    }
}

