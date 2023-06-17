using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Persistence;

namespace Persistence.Identity;
public class CustomUserStore : UserStore<User, Role, AppDbContext, Guid, IdentityUserClaim<Guid>, UserRole, IdentityUserLogin<Guid>, IdentityUserToken<Guid>, IdentityRoleClaim<Guid>>
{
    public CustomUserStore(AppDbContext context, IdentityErrorDescriber? describer = null) : base(context, describer)
    {
    }

    public override Task<User?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
    {
        //return base.FindByNameAsync(normalizedUserName, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        return Users.Include(u => u.UserRoles)
            .ThenInclude(r => r.Role)
            .FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName, cancellationToken);
    }

    public override Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken = default)
    {
        //return base.GetRolesAsync(user, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        return Task.FromResult<IList<string>>(user.UserRoles.Select(r => r.Role.Name!).ToList());
    }

    public override Task<User?> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        var id = ConvertIdFromString(userId);
        return Users.Include(u => u.UserRoles)
            .ThenInclude(r => r.Role)
            .FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId), cancellationToken);
    }

    public override Task<User?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        return Users.Include(u => u.UserRoles)
            .ThenInclude(r => r.Role)
            .SingleOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
    }
}
