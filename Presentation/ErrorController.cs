using System.Net;

using Microsoft.AspNetCore.Mvc;

namespace Presentation;

/*
 * Adds middleware to the pipeline that will catch exceptions, log them, and re-execute the request in an alternate pipeline.
 * Route: /Error
*/
public sealed class ErrorController : ControllerBase
{
    [Route("/Error")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult Error()
    {
        //throw new NotImplementedException();
        var problemDetails = new ProblemDetails
        {
            Status = (int)HttpStatusCode.InternalServerError,
            Title = "Internal Server Error",
            Detail = "An error occurred while processing your request.",
            Instance = HttpContext.Request.Path
        };
        return StatusCode((int)HttpStatusCode.InternalServerError, problemDetails);
    }
}
