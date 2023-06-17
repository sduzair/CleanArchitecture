using System.Reflection;
using System.Security.Claims;

using Application.Common.Security.Roles;

using Microsoft.AspNetCore.Authorization;
using Application.Common.Security.Policies;
using Persistence.Identity;

namespace Application.Common.Security.Requirements;

/// <summary>
/// Authorization requirement for <see cref="LogoutPolicy"/>
/// </summary>
public class LogoutAuthorizationRequirement : AuthorizationHandler<LogoutAuthorizationRequirement>, IAuthorizationRequirement
{
    public IReadOnlyList<string> Roles { get; init; }

    public string RequiredClaimType { get; init; }

    public LogoutAuthorizationRequirement()
    {
        Roles = GetRoleNames();
        RequiredClaimType = ClaimTypes.Email;
    }

    private static IReadOnlyList<string> GetRoleNames()
    {
        //Using Reflection to get all the roles
        var roleTypes = Assembly.GetAssembly(typeof(IRole))!.DefinedTypes
            .Where(t => t.IsAbstract && t.IsClass && t.GetInterfaces().Contains(typeof(IRole)));

        var roleTypeNames = roleTypes.Select(t => t.Name).ToList();

        return roleTypeNames.AsReadOnly();
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, LogoutAuthorizationRequirement requirement)
    {
        if (!context.User.HasClaim(MatchClaim))
        {
            return Task.CompletedTask;
        }

        if (HasAnyRequiredRole(context, requirement))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;

        bool MatchClaim(Claim obj)
        {
            return requirement.RequiredClaimType == obj.Type;
        }
    }

    private static bool HasAnyRequiredRole(AuthorizationHandlerContext context, LogoutAuthorizationRequirement requirement)
    {
        return requirement.Roles.Any(context.User.IsInRole) && !context.User.IsInRole(nameof(Visitor));
    }
}
