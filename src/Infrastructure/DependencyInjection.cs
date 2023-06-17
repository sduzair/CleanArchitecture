using Application.Common.Interfaces;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()) 
        {
            services.AddTransient<IEmailSender, MessageSender>();
            services.AddTransient<ISmsSender, MessageSender>();
        }

        services.AddScoped<ITimeProvider, UtcClock>();

        return services;
    }
}
