using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AntiSoft.SensoGraph.Domain.Abstractions;
using AntiSoft.SensoGraph.Domain.Entities;
using AntiSoft.SensoGraph.Domain.Repositories;
using AntiSoft.SensoGraph.Infrastructure.Startup;
using AntiSoft.SensoGraph.Infrastructure.Web;

namespace AntiSoft.SensoGraph.Infrastructure.DependencyInjection
{
    /// <summary>
    /// System specific dependencies.
    /// </summary>
    internal static class SystemModule
    {
        /// <summary>
        /// Register dependencies.
        /// </summary>
        /// <param name="services">Services.</param>
        /// <param name="configuration">Configuration.</param>
        public static void Register(IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<IJsonHelper, SystemTextJsonHelper>();
            services.AddSingleton<IChartRepository, SqLiteChartRepository>(p =>
                new SqLiteChartRepository(configuration["ConnectionStrings:AppDatabase"]));
            services.AddSingleton<MetricProvidersContainer>();

            var metricTypes = MetricsInitializer.GetAllMetricProviderType();
            foreach (var metricType in metricTypes)
            {
                services.AddTransient(metricType);
            }
        }
    }
}
