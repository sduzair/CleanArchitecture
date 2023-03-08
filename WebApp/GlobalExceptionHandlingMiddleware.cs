using System.Net;
using System.Text.Json;
using System.Xml.Linq;

namespace WebApp;

/*
 * This middleware is used to handle global exceptions in the pipeline and return a response to the client.
 * IoC container should be configured to register this middleware.
 */
internal sealed class GlobalExceptionHandlingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            throw; //comment this to handle exception
            //await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError; // 500 if unexpected

        string message;
        switch (context.Request.ContentType)
        {
            case "application/json":
                message = JsonSerializer.Serialize(new { error = exception.Message });
                context.Response.ContentType = "application/json";
                break;
            case "application/xml":
                var xml = new XElement("error", exception.Message);
                context.Response.ContentType = "application/xml";
                message = xml.ToString();
                break;
            default:
                context.Response.ContentType = "text/plain";
                message = exception.Message;
                break;
        }
        context.Response.StatusCode = (int)code;
        return context.Response.WriteAsync(message);
    }
}
