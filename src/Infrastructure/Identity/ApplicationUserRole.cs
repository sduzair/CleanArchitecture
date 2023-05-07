using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class ApplicationUserRole : IdentityUserRole<Guid>
{
    public virtual ApplicationUser ApplicationUser { get; set; } = null!;
    public virtual ApplicationRole ApplicationRole { get; set; } = null!;
}