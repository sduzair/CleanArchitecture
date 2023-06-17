using Microsoft.AspNetCore.Identity;

namespace Persistence.Identity;
public class Role : IdentityRole<Guid>
{
    public virtual List<UserRole> UserRoles { get; set; } = new();
}
