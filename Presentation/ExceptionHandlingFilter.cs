using System.Net;

using Application.Authentication;

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

        context.HttpContext.Response.StatusCode = context.Exception switch
        {
            UserExistsException => (int)HttpStatusCode.Conflict,//set status code to 409
            ApplicationException => (int)HttpStatusCode.BadRequest,//set status code to 400
            KeyNotFoundException => (int)HttpStatusCode.NotFound,//set status code to 404
            _ => (int)HttpStatusCode.InternalServerError,//set status code to 500
        };
        context.ExceptionHandled = false;
    }
}
