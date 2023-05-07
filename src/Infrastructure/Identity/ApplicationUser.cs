using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;
public class ApplicationUser : IdentityUser<Guid>
{
    public ApplicationUser(string userName, string email)
    {
        Email = email;
        UserName = userName;
    }
    public ApplicationUser(string userName)
    {
        Email = userName;
        UserName = userName;
    }
    //private readonly List<IdentityUserRole<Guid>> _userRoles = new();
    //public virtual IReadOnlyList<IdentityUserRole<Guid>> ApplicatoinUserRoles => _userRoles.AsReadOnly();
    public virtual List<ApplicationUserRole> ApplicationUserRoles { get; set; } = new();
}
