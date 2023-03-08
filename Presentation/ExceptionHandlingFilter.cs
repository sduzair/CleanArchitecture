using System.Net;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Presentation;
/*
 * Filter can be applied on controller or action level using annotation [ExceptionHandlingFilter]
 * Filter can be applied to all controllers using the AddControllers() and passing Action<MvcOptions>
 * Injections are not supported in filters
*/
internal sealed class ExceptionHandlingFilter : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is null) return;
        // problem details response with stack trace and other details
        context.Result = new ObjectResult(new ProblemDetails
        {
            Status = (int)HttpStatusCode.InternalServerError,
            Title = "Internal Server Error",
            Detail = context.Exception.Message,
            Instance = context.HttpContext.Request.Path
        })
        {
            ContentTypes = { "application/problem+json", "application/problem+xml" },
        };

        context.ExceptionHandled = true;
    }
}
