using System.Reflection;

using Application.Common.Security.Policies;
using Application.Common.Security.Requirements;
using Application.Common.Security.Schemes;
using Application.Common.Security.Schemes.Custom;
using Application.Identity;
using Application.Users;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Persistence;
using Persistence.Identity;

namespace Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        //Application services
        services.AddScoped<AuthService>();
        services.AddScoped<UserService>();

        services.AddSession(o => o.IdleTimeout = TimeSpan.FromMinutes(30));
        services.AddDistributedMemoryCache();

        AddIdentityAndCustomAuthenticationScheme(services, env);
        AddAuthorizationPolicies(services);

        return services;
    }
    private static void AddIdentityAndCustomAuthenticationScheme(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddIdentity<User, Role>(o =>
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
        .AddEntityFrameworkStores<AppDbContext>()
        .AddUserStore<CustomUserStore>()
        .AddUserManager<CustomUserManager>()
        .AddRoles<Role>()
        .AddRoleManager<CustomRoleManager>()
        .AddDefaultTokenProviders();

        services.AddAuthentication()
        .AddApplicationCookie<CustomAuthenticationHandler>(CustomAuthenticationDefaults.AuthenticationScheme, o => o.Events = new CookieAuthenticationEvents
        {
            OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync
        });

        services.ConfigureApplicationCookie(co => co.ForwardDefault = CustomAuthenticationDefaults.AuthenticationScheme);

        //validates security stamp and replaces user principle with updated claims every 5 min
        services.Configure<SecurityStampValidatorOptions>(o => o.ValidationInterval = env.IsDevelopment() ? TimeSpan.FromSeconds(1) : TimeSpan.FromMinutes(5));
    }
    private static void AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(o =>
        {
            o.AddPolicy(nameof(ProductAdminPolicy), p =>
            {
                p.RequireAuthenticatedUser();
                p.RequireRole(ProductAdminPolicy.Roles);
            });
            o.AddPolicy(nameof(ProductManagementPolicy), p =>
            {
                p.RequireAuthenticatedUser();
                p.RequireRole(ProductManagementPolicy.Roles);
            });
            o.AddPolicy(nameof(ProductViewPolicy), p =>
            {
                p.RequireAuthenticatedUser();
                p.RequireRole(ProductViewPolicy.Roles);
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
            o.AddPolicy(nameof(UserManagementPolicy), p =>
            {
                p.RequireAuthenticatedUser();
                p.RequireRole(UserManagementPolicy.Roles);
            });
            o.AddPolicy(nameof(LogoutPolicy), policy => policy.Requirements.Add(new LogoutAuthorizationRequirement()));
        });
    }

}
