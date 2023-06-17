using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using Persistence.Identity;

namespace Application.Identity;
public class CustomRoleManager : RoleManager<Role>
{
    public CustomRoleManager(IRoleStore<Role> store, IEnumerable<IRoleValidator<Role>> roleValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<Role>> logger) : base(store, roleValidators, keyNormalizer, errors, logger)
    {
    }

    //To reduce queries to database made by UserClaimsPrincipalFactory
    public override bool SupportsRoleClaims => false;
}
