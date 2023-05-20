using Application;
using Application.Auth;
using Application.Common.Security.Policies;
using Application.UserManager;

using Infrastructure.Common;
using Infrastructure.Identity;
using Infrastructure.Utilities;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddDbContext<ApplicationDbContext>(o =>
        {
            if (env.IsDevelopment())
            {
                o.EnableSensitiveDataLogging(true);
            }
        });
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddIdentity<ApplicationUser, ApplicationRole>(o =>
        {
            if (env.IsDevelopment())
            {
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 0;
                o.Password.RequireUppercase = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireDigit = false;
            }
        })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddUserStore<ApplicationUserStore>()
            .AddUserManager<ApplicationUserManager>()
            .AddRoles<ApplicationRole>()
            .AddRoleManager<ApplicationRoleManager>()
            .AddDefaultTokenProviders();

        //validates security stamp and replaces principle with updated claims every 5 min
        services.Configure<SecurityStampValidatorOptions>(o => o.ValidationInterval = env.IsDevelopment() ? TimeSpan.FromSeconds(1) : TimeSpan.FromMinutes(5));

        services.AddAuthorization(o =>
        {
            o.AddPolicy(nameof(ProductsAdminPolicy), p =>
            {
                p.RequireAuthenticatedUser();
                p.RequireRole(ProductsAdminPolicy.Roles);
            });
            o.AddPolicy(nameof(ProductsManagementPolicy), p =>
            {
                p.RequireAuthenticatedUser();
                p.RequireRole(ProductsManagementPolicy.Roles);
            });
            o.AddPolicy(nameof(ProductsViewPolicy), p =>
            {
                p.RequireAuthenticatedUser();
                p.RequireRole(ProductsViewPolicy.Roles);
            });
            o.AddPolicy(nameof(CartPolicy), p =>
            {
                p.RequireAuthenticatedUser();
                p.RequireRole(CartPolicy.Roles);
            });
            o.AddPolicy(nameof(CustomerPolicy), p =>
            {
                p.RequireAuthenticatedUser();
                p.RequireRole(CustomerPolicy.Roles);
            });
        });

        //services.ConfigureApplicationCookie(co => co.LoginPath = "/Identity/Account/Login");

        //Application services
        services.AddScoped<Application.Common.Interfaces.ITimeProvider, UtcClock>();

        services.AddTransient<IEmailSender, MessageSender>();
        services.AddTransient<ISmsSender, MessageSender>();

        services.AddScoped<IApplicationAuthenticationService, ApplicationAuthenticationService>();
        services.AddScoped<IApplicationAuthorizationService, ApplicationAuthorizationService>();
        services.AddScoped<IApplicationUserService, ApplicationUserService>();

        services.AddDistributedMemoryCache();
        services.AddSession(o => o.IdleTimeout = TimeSpan.FromMinutes(30));

        return services;
    }
}
