using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VStarCameraZone.Services;

namespace VStarCameraZone.Infrastructure.DependencyInjection;

/// <summary>
/// Application specific dependencies.
/// </summary>
internal static class ApplicationModule
{
    /// <summary>
    /// Register dependencies.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="configuration">App configuration.</param>
    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<CamerasFinder>();
    }
}
