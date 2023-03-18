
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Presentation;
public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers(options => options.RespectBrowserAcceptHeader = true)
            .AddXmlSerializerFormatters();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddProblemDetails();
        services.AddTransient<ContentNegotiationMiddleware>();
        return services;
    }
}

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UsePresentation(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (!env.IsDevelopment())
        {
            app.UseExceptionHandler();  //for unhandled exceptions in production
            app.UseHsts();
        }

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<ContentNegotiationMiddleware>();
        app.UseExceptionHandler("/Error");
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
        return app;
    }
}
