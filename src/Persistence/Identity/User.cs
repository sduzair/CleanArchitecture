using Microsoft.AspNetCore.Identity;

namespace Persistence.Identity;
public class User : IdentityUser<Guid>
{
    public User(string userName, string email)
    {
        Email = email;
        UserName = userName;
    }
    public User(string userName)
    {
        Email = userName;
        UserName = userName;
    }
    //private readonly List<IdentityUserRole<Guid>> _userRoles = new();
    //public virtual IReadOnlyList<IdentityUserRole<Guid>> ApplicatoinUserRoles => _userRoles.AsReadOnly();
    public virtual List<UserRole> UserRoles { get; set; } = new();
}
