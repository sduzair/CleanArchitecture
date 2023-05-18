using Application.Common.Interfaces;

using FluentResults.Extensions.AspNetCore;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Presentation.Common;
using Presentation.Middleware;
using Presentation.Services;

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

        services.AddScoped<ApplicationAspNetCoreResultEndpointProfile>();

        services.AddScoped<ICurrentUserService, CurrentUserService>();

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
