using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;
public class ApplicationRole : IdentityRole<Guid>
{
    public virtual List<ApplicationUserRole> ApplicatoinUserRoles { get; set; } = new();
}
