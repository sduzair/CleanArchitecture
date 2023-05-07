﻿using Application;
using Application.Auth;
using Application.Products;

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
        services.AddDbContext<ApplicationDbContext>();
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddIdentity<ApplicationUser, ApplicationRole>(sa =>
        {
            if (env.IsDevelopment())
            {
                sa.Password.RequireNonAlphanumeric = false;
                sa.Password.RequiredLength = 0;
                sa.Password.RequireUppercase = false;
                sa.Password.RequireLowercase = false;
                sa.Password.RequireDigit = false;
            }
        })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddUserStore<ApplicationUserStore>()
            .AddUserManager<ApplicationUserManager>()
            .AddRoles<ApplicationRole>()
            .AddDefaultTokenProviders();

        services.AddAuthorization(o =>
        {
            o.AddPolicy(ProductAdminPolicy.PolicyName, p =>
            {
                p.RequireAuthenticatedUser();
                p.RequireRole(ProductAdminPolicy.Roles);
            });
            o.AddPolicy(ProductManagementPolicy.PolicyName, p =>
            {
                p.RequireAuthenticatedUser();
                p.RequireRole(ProductManagementPolicy.Roles);
            });
            o.AddPolicy(ProductViewPolicy.PolicyName, p =>
            {
                p.RequireAuthenticatedUser();
                p.RequireRole(ProductViewPolicy.Roles);
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

        return services;
    }
}
