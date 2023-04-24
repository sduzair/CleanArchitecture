
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using Presentation.Middleware;

namespace Presentation;
public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers(options => options.RespectBrowserAcceptHeader = true)
            .AddXmlSerializerFormatters();
        services.AddProblemDetails();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddTransient<ContentNegotiationMiddleware>();
        return services;
    }
}

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UsePresentation(this IApplicationBuilder app)
    {
        app.UseMiddleware<ContentNegotiationMiddleware>();
        return app;
    }
}
