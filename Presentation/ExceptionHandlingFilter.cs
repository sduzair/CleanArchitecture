using System.Net;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Presentation;
/*
 * Filters can be applied on controller or action level using annotation [ExceptionHandlingFilter]
 * Filters can be applied to all controllers using the AddControllers() and passing Action<MvcOptions>
 * Injections not done in filters
*/
internal sealed class ExceptionHandlingFilter : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is null) return;
        // problem details response with stack trace and other details - (uncomment to use)
        context.Result = new ObjectResult(new ProblemDetails
        {
            Status = (int)HttpStatusCode.InternalServerError,
            Title = "Internal Server Error asdfj",  //status code is set by ObjectResult
            Detail = context.Exception.Message,
            Instance = context.HttpContext.Request.Path
        })
        {
            ContentTypes = { "application/problem+json", "application/problem+xml" },
        };

        //set ExceptionHandled to true to prevent other filters from handling the exception
        context.ExceptionHandled = true;
    }
}
