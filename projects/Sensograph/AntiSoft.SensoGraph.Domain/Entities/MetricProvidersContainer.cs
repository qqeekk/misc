using System;
using System.Collections.Generic;
using AntiSoft.SensoGraph.Domain.Abstractions;

namespace AntiSoft.SensoGraph.Domain.Entities
{
    /// <summary>
    /// Container for metric providers.
    /// </summary>
    public sealed class MetricProvidersContainer : IDisposable
    {
        private readonly Dictionary<string, IMetricProvider> allMetricProviders = new ();

        /// <summary>
        /// Register new metrics provider with the specific code.
        /// </summary>
        /// <param name="code">User code.</param>
        /// <param name="metricProvider">Metric provider.</param>
        public void Register(string code, IMetricProvider metricProvider)
        {
            allMetricProviders[code] = metricProvider;
        }

        /// <summary>
        /// Get all metric providers.
        /// </summary>
        /// <returns>Metric providers.</returns>
        public IDictionary<string, IMetricProvider> GetAll() => allMetricProviders;

        /// <summary>
        /// Get metric provider by code.
        /// </summary>
        /// <param name="code">Code.</param>
        /// <returns>Metric provider.</returns>
        public IMetricProvider GetByCode(string code) => allMetricProviders[code];

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var metricProvider in allMetricProviders)
            {
                var metricProviderDisposable = metricProvider.Value as IDisposable;
                metricProviderDisposable?.Dispose();
            }
            allMetricProviders.Clear();
        }
    }
}
