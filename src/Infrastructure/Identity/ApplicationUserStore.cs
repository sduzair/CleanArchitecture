﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity;
internal class ApplicationUserStore : UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid, IdentityUserClaim<Guid>, ApplicationUserRole, IdentityUserLogin<Guid>, IdentityUserToken<Guid>, IdentityRoleClaim<Guid>>
{
    public ApplicationUserStore(ApplicationDbContext context, IdentityErrorDescriber? describer = null) : base(context, describer)
    {
    }

    public override Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
    {
        //return base.FindByNameAsync(normalizedUserName, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        return Users.Include(u => u.ApplicationUserRoles)
            .ThenInclude(r => r.ApplicationRole)
            .FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName, cancellationToken);
    }

    public override Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        //return base.GetRolesAsync(user, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        return Task.FromResult<IList<string>>(user.ApplicationUserRoles.Select(r => r.ApplicationRole.Name!).ToList());
    }

    public override Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        var id = ConvertIdFromString(userId);
        return Users.Include(u => u.ApplicationUserRoles)
            .ThenInclude(r => r.ApplicationRole)
            .FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId), cancellationToken);
    }

    public override Task<ApplicationUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        return Users.Include(u => u.ApplicationUserRoles)
            .ThenInclude(r => r.ApplicationRole)
            .SingleOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
    }
}
