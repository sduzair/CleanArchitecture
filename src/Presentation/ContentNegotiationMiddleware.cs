using System.Net;
using System.Text.Json;
using System.Xml.Linq;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Presentation;

/*
 * This middleware is used to handle/intercept exceptions in the pipeline
 * IoC container should be configured to register this middleware as Transient
 */
internal sealed class ContentNegotiationMiddleware : IMiddleware
{
    private readonly ProblemDetailsFactory _problemDetailsFactory;

    public ContentNegotiationMiddleware(ProblemDetailsFactory problemDetailsFactory)
    {
        _problemDetailsFactory = problemDetailsFactory;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        await ContentNegotiationFilterAsync(context);
        //check if response is already set by the filter above and return if so to avoid double response
        if (context.Response.HasStarted) { return; }
        await next(context);
    }

    private async Task ContentNegotiationFilterAsync(HttpContext context)
    {
        //content negotiation
        if (!context.Request.Headers.TryGetValue("Accept", out _))
        {
            await NotAcceptableProblemDetailsResponse(context);
            return;
        }
    }

    private Task NotAcceptableProblemDetailsResponse(HttpContext context)
    {
        var problemDetails = _problemDetailsFactory.CreateProblemDetails(
            httpContext: context,
            statusCode: (int)HttpStatusCode.NotAcceptable);

        ObjectResult objectResult = new(problemDetails);
        var message = JsonSerializer.Serialize(objectResult.Value!);

        return context.Response.WriteAsync(message);
    }
}
