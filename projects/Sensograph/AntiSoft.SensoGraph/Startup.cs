using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AntiSoft.SensoGraph.Infrastructure.Middlewares;
using AntiSoft.SensoGraph.Infrastructure.Settings;
using AntiSoft.SensoGraph.Infrastructure.Startup;

namespace AntiSoft.SensoGraph
{
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
            // Health check.
            services.AddHealthChecks();

            // Localization.
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new ("en-US"),
                    new ("ru-RU")
                };

                options.DefaultRequestCulture = new RequestCulture(culture: "ru-RU", uiCulture: "ru-RU");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            // MVC.
            services
                .AddControllersWithViews()
                .AddJsonOptions(new JsonOptionsSetup().Setup)
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();
            services.Configure<ApiBehaviorOptions>(
                new ApiBehaviorOptionsSetup().Setup);

            // Database.
            services.AddAsyncInitializer<DatabaseInitializer>();

            // Logging.
            services.AddLogging(new LoggingOptionsSetup(configuration, environment).Setup);

            // Application settings.
            services.Configure<AppSettings>(configuration.GetSection("Application"));
            services.AddAsyncInitializer<MetricsInitializer>();

            // HTTP client.
            services.AddHttpClient();

            // Other dependencies.
            Infrastructure.DependencyInjection.SystemModule.Register(
                services,
                configuration);

            // Compression.
            services.AddResponseCompression();
        }

        /// <summary>
        /// Configure web application.
        /// </summary>
        /// <param name="app">Application builder.</param>
        public void Configure(IApplicationBuilder app)
        {
            // Compression.
            app.UseResponseCompression();

            // Custom middlewares.
            app.UseMiddleware<ApiExceptionMiddleware>();

            // MVC.
            app.UseRouting();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });
            app.UseRequestLocalization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health-check",
                    new HealthCheckOptionsSetup().Setup(
                        new HealthCheckOptions())
                );
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllers();
            });
        }
    }
}
