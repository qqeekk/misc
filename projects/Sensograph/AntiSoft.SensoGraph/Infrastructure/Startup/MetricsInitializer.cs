using System;
using System.Linq;
using System.Threading.Tasks;
using Extensions.Hosting.AsyncInitialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AntiSoft.SensoGraph.Domain.Abstractions;
using AntiSoft.SensoGraph.Domain.Entities;
using AntiSoft.SensoGraph.Infrastructure.Settings;

namespace AntiSoft.SensoGraph.Infrastructure.Startup
{
    /// <summary>
    /// Metrics initializer.
    /// </summary>
    public class MetricsInitializer : IAsyncInitializer
    {
        private readonly MetricProvidersContainer metricProvidersContainer;
        private readonly IOptions<AppSettings> settings;
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<MetricsInitializer> logger;

        public MetricsInitializer(
            MetricProvidersContainer metricProvidersContainer,
            IOptions<AppSettings> settings,
            IServiceProvider serviceProvider,
            ILogger<MetricsInitializer> logger)
        {
            this.metricProvidersContainer = metricProvidersContainer;
            this.settings = settings;
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        internal static Type[] GetAllMetricProviderType()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(IMetricProvider).IsAssignableFrom(p) && p.IsClass)
                .ToArray();
        }

        /// <inheritdoc />
        public async Task InitializeAsync()
        {
            var metricTypes = GetAllMetricProviderType();
            foreach (var valueMetric in settings.Value.Metrics)
            {
                if (!valueMetric.Enabled)
                {
                    continue;
                }
                var type = metricTypes.FirstOrDefault(mt =>
                    mt.Name.Equals(valueMetric.Class, StringComparison.OrdinalIgnoreCase));
                if (type == null)
                {
                    throw new InvalidOperationException($"Cannot resolve metric type {valueMetric.Class}.");
                }
                var metricProvider = (IMetricProvider) serviceProvider.GetService(type);
                await metricProvider.Initialize(valueMetric.Options);
                metricProvidersContainer.Register(valueMetric.Code, metricProvider);
                logger.LogInformation("Add metric provider {code}: {name}.", valueMetric.Code, type.Name);
            }
        }
    }
}
