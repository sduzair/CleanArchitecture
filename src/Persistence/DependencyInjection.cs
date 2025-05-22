using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IWebHostEnvironment webHostEnvironment)
    {
        services.AddDbContext<AppDbContext>(o =>
        {
            if (webHostEnvironment.IsDevelopment())
            {
                o.EnableSensitiveDataLogging();
            }
            o.UseSqlite($"Data Source={Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cleanarchitecture.db")}");
        });

        return services;
    }
}
