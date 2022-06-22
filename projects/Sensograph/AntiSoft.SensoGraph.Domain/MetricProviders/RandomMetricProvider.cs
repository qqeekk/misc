using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AntiSoft.SensoGraph.Domain.Abstractions;

namespace AntiSoft.SensoGraph.Domain.MetricProviders
{
    /// <summary>
    /// Provides random value between -100 and 100.
    /// </summary>
    public class RandomMetricProvider : IMetricProvider
    {
        private readonly Random random = new();

        /// <inheritdoc />
        public string UnitOfMeasurement => "tst";

        /// <inheritdoc />
        public Task Initialize(IDictionary<string, string> options) => Task.CompletedTask;

        /// <inheritdoc />
        public Task<decimal> GetValue(CancellationToken cancellationToken = default) =>
            Task.FromResult((decimal)random.Next(-100, 100));
    }
}
