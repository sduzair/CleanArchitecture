using System.Net;

using Application.Authentication;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Presentation;

/*
 * Adds middleware to the pipeline that will catch exceptions, log them, and re-execute the request in an alternate pipeline.
*/
[ApiController]
public sealed class ErrorController : ControllerBase
{
    [Route("/Error")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult HandleErrorDevelopment()
    {
        var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>()!;

        var problemDetails = new ProblemDetails
        {
            Title = exceptionHandlerFeature.Error.Message,
            Status = exceptionHandlerFeature.Error switch
            {
                UserExistsException _ => (int)HttpStatusCode.Conflict,
                ApplicationException _ => (int)HttpStatusCode.BadRequest,
                KeyNotFoundException _ => (int)HttpStatusCode.NotFound,
                _ => (int?)(int)HttpStatusCode.InternalServerError,
            }
        };
        return new ObjectResult(problemDetails)
        {
            ContentTypes = { "application/problem+json", "application/problem+xml" },
            
        };
    }
}
