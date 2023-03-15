using System.Net;

using Application.Authentication;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Presentation;

/*
 * Adds middleware to the pipeline that will catch exceptions, log them, and re-execute the request in an alternate pipeline.
 * Route: /Error
*/
//app.UseExceptionHandler("/Error");    //custom exception handler
[ApiController]
public sealed class ErrorController : ControllerBase
{
    [Route("/Error")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult HandleError()
    {
        return Problem();
    }

    /*
     * Sufficient to add AddProblemDetails() to service container for purposes of displaying error in ProblemDetails format (development)
     * This only demonstrates use of custom ExceptionHandler.
    */
    //app.UseExceptionHandler("/Error-Development");    //custom exception handler
    [Route("/Error-Development")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult HandleErrorDevelopment()
    {
        var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>()!;

        var problemDetails = new ProblemDetails
        {
            Title = exceptionHandlerFeature.Error.Message,
            Detail = exceptionHandlerFeature.Error.StackTrace,
        };

        switch (exceptionHandlerFeature.Error)
        {
            case UserExistsException _:
                problemDetails.Status = (int)HttpStatusCode.Conflict;
                break;
            case ApplicationException _:
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                break;
            case KeyNotFoundException _:
                problemDetails.Status = (int)HttpStatusCode.NotFound;
                break;
            default:
                problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                break;
        }

        return new ObjectResult(problemDetails)
        {
            ContentTypes = { "application/problem+json", "application/problem+xml" },
        };
    }
}
