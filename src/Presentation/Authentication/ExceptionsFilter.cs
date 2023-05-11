using System.Net;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Presentation.Authentication;
/*
 * Filters can be applied on controller or action level using annotation [ExceptionsFilter]
 * Filters can be applied to all controllers using the AddControllers() and passing Action<MvcOptions>
 * Dependency Injections not possible in simple filters
*/
internal sealed class ExceptionsFilter : ExceptionFilterAttribute, IExceptionFilter
{
    private readonly ProblemDetailsFactory _problemDetailsFactory;
    private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;

    public ExceptionsFilter(ProblemDetailsFactory problemDetailsFactory)
    {
        _problemDetailsFactory = problemDetailsFactory;
        _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>()
        {
            { typeof(InvalidOperationException), ExceptionHandler }
        };
    }

    private void ExceptionHandler(ExceptionContext context)
    {
        context.Result = new ObjectResult(_problemDetailsFactory.CreateProblemDetails(
            httpContext: context.HttpContext,
            statusCode: (int)HttpStatusCode.InternalServerError,
            title: context.Exception.GetType().Name,
            detail: context.Exception.Message));

        context.ExceptionHandled |= true;
    }

    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is null) return;
        if (_exceptionHandlers.TryGetValue(context.Exception.GetType(), out Action<ExceptionContext>? handler))
        {
            if (handler is null) return;
            handler(context);
        }
        else
        {
            context.ExceptionHandled = false;
        }

        base.OnException(context);
    }
}
