using System.Net;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Presentation.Filters;
/*
 * Filters can be applied on controller or action level using annotation [AuthExceptionHandlingFilter]
 * Filters can be applied to all controllers using the AddControllers() and passing Action<MvcOptions>
 * Dependency Injections not possible in simple filters
*/
internal sealed class AuthExceptionHandlingFilter : ExceptionFilterAttribute, IExceptionFilter
{
    private readonly ProblemDetailsFactory _problemDetailsFactory;
    private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;

    public AuthExceptionHandlingFilter(ProblemDetailsFactory problemDetailsFactory)
    {
        _problemDetailsFactory = problemDetailsFactory;
        _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>()
        {
            { typeof(Application.Auth.Exceptions.UserExistsException), HandleUserExistsException },
            { typeof(Application.Auth.Exceptions.EmailConfirmationException), HandleEmailConfirmationException },
            { typeof(Application.Auth.Exceptions.UserNotFoundException), HandleUserNotFoundException },
            { typeof(Application.Auth.Exceptions.IncorrectPasswordException), HandleIncorrectPasswordException  }
        };
    }

    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is null) return;
        if (_exceptionHandlers.TryGetValue(context.Exception.GetType(), out Action<ExceptionContext>? value))
        {
            if (value is null) return;
            value(context);
        }
        else
        {
            context.ExceptionHandled = false;
        }

        base.OnException(context);
    }

    private void HandleUserExistsException(ExceptionContext context)
    {
        context.Result = new ObjectResult(_problemDetailsFactory.CreateProblemDetails(httpContext: context.HttpContext,
            statusCode: (int)HttpStatusCode.Conflict,
            title: context.Exception.Message));

        context.ExceptionHandled |= true;
    }

    private void HandleEmailConfirmationException(ExceptionContext context)
    {
        context.Result = new ObjectResult(_problemDetailsFactory.CreateProblemDetails(httpContext: context.HttpContext,
            statusCode: (int)HttpStatusCode.Forbidden,
            title: context.Exception.Message));

        context.ExceptionHandled |= true;
    }

    private void HandleUserNotFoundException(ExceptionContext context)
    {
        context.Result = new ObjectResult(_problemDetailsFactory.CreateProblemDetails(httpContext: context.HttpContext,
            statusCode: (int)HttpStatusCode.NotFound,
            title: context.Exception.Message));

        context.ExceptionHandled |= true;
    }

    private void HandleIncorrectPasswordException(ExceptionContext context)
    {
        context.Result = new ObjectResult(_problemDetailsFactory.CreateProblemDetails(httpContext: context.HttpContext,
            statusCode: (int)HttpStatusCode.Unauthorized,
            title: context.Exception.Message));

        context.ExceptionHandled |= true;
    }
}
