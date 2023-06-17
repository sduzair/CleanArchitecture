using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddDbContext<AppDbContext>(o =>
        {
            if (env.IsDevelopment())
            {
                o.EnableSensitiveDataLogging(true);
            }
        });


        return services;
    }

}
