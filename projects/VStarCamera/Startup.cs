using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VStarCameraZone.Entities;
using VStarCameraZone.Infrastructure.DependencyInjection;
using VStarCameraZone.Infrastructure.Startup;

namespace VStarCameraZone;

/// <summary>
/// Entry point for ASP.NET Core app.
/// </summary>
public class Startup
{
    private readonly IConfiguration configuration;

    private readonly IWebHostEnvironment environment;

    /// <summary>
    /// Entry point for web application.
    /// </summary>
    /// <param name="configuration">Global configuration.</param>
    /// <param name="environment">Environment.</param>
    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        this.configuration = configuration;
        this.environment = environment;
    }

    /// <summary>
    /// Configure application services on startup.
    /// </summary>
    /// <param name="services">Services to configure.</param>
    public void ConfigureServices(IServiceCollection services)
    {
        // MVC.
        services
            .AddControllersWithViews()
            .AddJsonOptions(new JsonOptionsSetup().Setup);

        // Logging.
        services.AddLogging(new LoggingOptionsSetup(configuration, environment).Setup);

        // Application settings.
        services.Configure<CredentialsDictionary>(
            configuration.GetSection("Credentials"));

        // Other dependencies.
        ApplicationModule.Register(services, configuration);

        // Compression.
        services.AddResponseCompression();

        services.AddCliCommands();
    }

    /// <summary>
    /// Configure web application.
    /// </summary>
    /// <param name="app">Application builder.</param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Compression.
        app.UseResponseCompression();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }
        app.UseStaticFiles();

        // MVC.
        app.UseRouting();
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.All
        });
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            endpoints.MapControllers();
        });
    }
}
