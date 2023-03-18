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

        var (statusCode, message) = exceptionHandlerFeature.Error switch
        {
            ApplicationException _ => ((int)HttpStatusCode.BadRequest, exceptionHandlerFeature.Error.Message),
            KeyNotFoundException _ => ((int)HttpStatusCode.NotFound, exceptionHandlerFeature.Error.Message),
            _ => throw exceptionHandlerFeature.Error,
        };

        return Problem(statusCode: statusCode, title: message);
    }
}
