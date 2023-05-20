using System.Reflection;

using Application.Common.Security.Roles;

using Infrastructure.Utilities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Identity.EFConfigurations;

internal class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        /// Each <see cref="ApplicationRole"/> can have many entries in the <see cref="ApplicationUserRole"/> join table
        builder.HasMany(e => e.ApplicatoinUserRoles)
            .WithOne(e => e.ApplicationRole)
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();

        var roles = GetRoleNames();

        builder.HasData(roles.Select(x => new ApplicationRole { Id = GuidHelper.GenerateDeterministicGuid(x), Name = x, NormalizedName = x.ToUpper() }));
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

