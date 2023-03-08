using Infrastructure.Authentication;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        //Application services
        services.AddScoped<Application.Interfaces.IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<Application.Authentication.Interfaces.IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<Application.Interfaces.IUserRepository<Domain.Entities.User>, UserRepository>();
        //Local services
        services.AddOptions<JwtTokenOptions>()
            .Bind(configuration.GetSection(JwtTokenOptions.KeyName), options => options.ErrorOnUnknownConfiguration = true)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddScoped<JwtTokenHandler>();
        return services;
    }
}
