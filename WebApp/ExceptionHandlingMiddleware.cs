using System.Net;
using System.Text.Json;
using System.Xml.Linq;

using Application.Authentication;

namespace WebApp;

/*
 * This middleware is used to handle/intercept exceptions in the pipeline
 * IoC container should be configured to register this middleware.
 */
//builder.Services.AddTransient<ExceptionHandlingMiddleware>();
//app.UseMiddleware<ExceptionHandlingMiddleware>();
internal sealed class ExceptionHandlingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            //await HandleExceptionAsync(context, ex);
            InterceptException(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError; // 500 if unexpected

        string message;

        //switch on exception and return appropriate response message and code

        message = JsonSerializer.Serialize(new { error = exception.Message });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        return context.Response.WriteAsync(message);
    }

    private static void InterceptException(HttpContext context, Exception exception)
    {
        switch (exception)
        {
            case UserExistsException _:
                //do something
                break;
            case ApplicationException _:
                //do something
                break;
            case KeyNotFoundException _:
                //do something
                break;
            default:
                //do something
                break;
        }
    }
}
