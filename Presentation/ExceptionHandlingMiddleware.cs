using System.Net;
using System.Text.Json;
using System.Xml.Linq;

using Application.Authentication;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Presentation;

/*
 * This middleware is used to handle/intercept exceptions in the pipeline
 * IoC container should be configured to register this middleware as Transient
 */
internal sealed class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ProblemDetailsFactory _problemDetailsFactory;

    public ExceptionHandlingMiddleware(ProblemDetailsFactory problemDetailsFactory)
    {
        _problemDetailsFactory = problemDetailsFactory;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var problemDetails = _problemDetailsFactory.CreateProblemDetails(
            httpContext: context,
            statusCode: (int)HttpStatusCode.InternalServerError);

        context.Response.ContentType = "application/json";
        var message = JsonSerializer.Serialize(new ObjectResult(problemDetails));
        return context.Response.WriteAsync(message);
    }
}
