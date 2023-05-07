using System.Reflection;

using Application.Auth;
using Application.Common.Errors;
using Application.Common.Interfaces;
using Application.Common.Security;

using FluentResults;

using MediatR;

namespace Application.Common.Behaviours;

public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : ResultBase, new()
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationAuthorizationService _applicationAuthorizationService;

    public AuthorizationBehaviour(ICurrentUserService currentUserService, IApplicationAuthorizationService applicationAuthorizationService)
    {
        _currentUserService = currentUserService;
        _applicationAuthorizationService = applicationAuthorizationService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        IEnumerable<ApplicationAuthorizeAttribute> authorizeAttributes = request.GetType().GetCustomAttributes<ApplicationAuthorizeAttribute>() ?? new List<ApplicationAuthorizeAttribute>();

        bool authorized = await RoleBasedAuthorizationHandler(authorizeAttributes) && await PolicyBasedAuthorizationHandler(authorizeAttributes);

        if (!authorized)
        {
            //TODO make this a 403 forbidden exception
            var result = new TResponse();
            result.Reasons.Add(new ForbiddenAccessError(_currentUserService.UserId));
            return result;
        }

        return await next();
    }

    private async Task<bool> PolicyBasedAuthorizationHandler(IEnumerable<ApplicationAuthorizeAttribute> authorizeAttributes)
    {
        //Policy-based authorization
        var authorizationAttributesWithPolicies = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Policy));
        if (!authorizationAttributesWithPolicies.Any())
        {
            return true;
        }
        foreach (var policy in authorizationAttributesWithPolicies.Select(a => a.Policy!))
        {
            var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(_currentUserService.User, policy);
            if (!authorizationResult.Succeeded)
            {
                return false;
            }
        }
        return true;
    }

    private async Task<bool> RoleBasedAuthorizationHandler(IEnumerable<ApplicationAuthorizeAttribute> authorizeAttributes)
    {
        //Role-based authorization
        IEnumerable<ApplicationAuthorizeAttribute> authorizeAttributesWithRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));

        if (!authorizeAttributesWithRoles.Any())
        {
            return true;
        }

        foreach (var roles in authorizeAttributesWithRoles.Select(a => a.Roles!.Split(',')))
        {
            foreach (var role in roles)
            {
                var isInRole = await _applicationAuthorizationService.IsInRoleAsync(_currentUserService.UserId, role.Trim());
                if (isInRole)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
