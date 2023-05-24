using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Application.Common.Security.Schemes;

/// <summary>
/// Extension methods to configure cookie authentication.
/// </summary>
public static class ApplicationCookieExtensions
{
    /// <summary>
    /// Adds cookie authentication to <see cref="AuthenticationBuilder"/> using the specified scheme.
    /// <para>
    /// Cookie authentication uses a HTTP cookie persisted in the client to perform authentication.
    /// </para>
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="authenticationScheme">The authentication scheme.</param>
    /// <param name="configureOptions">A delegate to configure <see cref="CookieAuthenticationOptions"/>.</param>
    /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
    public static AuthenticationBuilder AddApplicationCookie<THandler>(this AuthenticationBuilder builder, string authenticationScheme, Action<CookieAuthenticationOptions> configureOptions)
        where THandler : AuthenticationHandler<CookieAuthenticationOptions>
    {
        return builder.AddApplicationCookie<THandler>(authenticationScheme, displayName: null, configureOptions: configureOptions);
    }

    /// <summary>
    /// Adds cookie authentication to <see cref="AuthenticationBuilder"/> using the specified scheme.
    /// <para>
    /// Cookie authentication uses a HTTP cookie persisted in the client to perform authentication.
    /// </para>
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="authenticationScheme">The authentication scheme.</param>
    /// <param name="displayName">A display name for the authentication handler.</param>
    /// <param name="configureOptions">A delegate to configure <see cref="CookieAuthenticationOptions"/>.</param>
    /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
    public static AuthenticationBuilder AddApplicationCookie<THandler>(this AuthenticationBuilder builder, string authenticationScheme, string? displayName, Action<CookieAuthenticationOptions> configureOptions)
        where THandler : AuthenticationHandler<CookieAuthenticationOptions>
    {
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<CookieAuthenticationOptions>, PostConfigureCookieAuthenticationOptions>());
        builder.Services.AddOptions<CookieAuthenticationOptions>(authenticationScheme).Validate(o => o.Cookie.Expiration == null, "Cookie.Expiration is ignored, use ExpireTimeSpan instead.");
        return builder.AddScheme<CookieAuthenticationOptions, THandler>(authenticationScheme, displayName, configureOptions);
    }
}
