using System.Security.Claims;

using Microsoft.AspNetCore.Identity;

namespace Application.UserManager;
public interface IApplicationUserService
{
    Task<IdentityResult> AddUserToRoleAsync(string email, string roleName);
    Task<IdentityResult> DeleteUserAsync(ClaimsPrincipal claimsPrinciple);
    Task<IdentityResult> DeleteUserAsync(string email);
    Task<(IdentityResult, IReadOnlyList<string>?)> GetRolesForUserAsync(string email);
    Task<IReadOnlyList<string>> GetUsersInRoleAsync(string roleName);
    Task<IdentityResult> RemoveUserFromRoleAsync(string email, string roleName);
}
