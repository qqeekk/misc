using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AntiSoft.SensoGraph.Domain.Abstractions
{
    /// <summary>
    /// Metrics provider.
    /// </summary>
    public interface IMetricProvider
    {
        /// <summary>
        /// Unit of measure.
        /// </summary>
        string UnitOfMeasurement { get; }

        /// <summary>
        /// Initialize metric provider. The method calls first before any other method.
        /// </summary>
        /// <param name="options">Options.</param>
        /// <returns>Awaitable task.</returns>
        Task Initialize(IDictionary<string, string> options);

        /// <summary>
        /// Get value for now.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>Value.</returns>
        Task<decimal> GetValue(CancellationToken cancellationToken = default);
    }
}
