using System.Net;

using Application.Authentication.Exceptions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Presentation;
/*
 * Filters can be applied on controller or action level using annotation [AuthenticationExceptionHandlingFilter]
 * Filters can be applied to all controllers using the AddControllers() and passing Action<MvcOptions>
 * Injections not done in filters
*/
internal sealed class AuthenticationExceptionHandlingFilter : IExceptionFilter
{
    private readonly ProblemDetailsFactory _problemDetailsFactory;

    public AuthenticationExceptionHandlingFilter(ProblemDetailsFactory problemDetailsFactory)
    {
        _problemDetailsFactory = problemDetailsFactory;
    }

    public void OnException(ExceptionContext context)
    {
        if (context.Exception is null) return;

        int statusCode;
        string message;

        if (context.Exception is UserExistsException) { statusCode = (int)HttpStatusCode.Conflict; }
        else if (context.Exception is UserNotVerifiedException) { statusCode = (int)HttpStatusCode.Forbidden; }
        else if (context.Exception is UserNotFoundException) { statusCode = (int)HttpStatusCode.NotFound; }
        else
        {
            context.ExceptionHandled = false;
            return;
        }

        message = context.Exception.Message;
        context.Result = new ObjectResult(_problemDetailsFactory.CreateProblemDetails(httpContext: context.HttpContext,
            statusCode: statusCode,
            title: message));
    }
}
