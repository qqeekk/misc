using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AntiSoft.SensoGraph.Domain.Entities;

namespace AntiSoft.SensoGraph.Domain.Abstractions
{
    /// <summary>
    /// Persistent time-series data for a chart.
    /// </summary>
    public interface IChartRepository
    {
        /// <summary>
        /// Get data for period.
        /// </summary>
        /// <param name="start">Period start.</param>
        /// <param name="end">Period end.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>Chart items.</returns>
        Task<IList<ChartItem>> GetForPeriod(DateTime start, DateTime end,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Add new chart item.
        /// </summary>
        /// <param name="item">Chart item.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        Task Add(ChartItem item, CancellationToken cancellationToken = default);

        /// <summary>
        /// Remove old records.
        /// </summary>
        /// <param name="minDateToKeep">Minimum date to keep records. Older records will be removed.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>Number of removed records.</returns>
        Task<int> RemoveOldRecords(DateTime minDateToKeep, CancellationToken cancellationToken = default);

        /// <summary>
        /// Initialize data storage. It will be called at the repository creation.
        /// </summary>
        /// <param name="metricCodes">All metric codes.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        Task Initialize(string[] metricCodes, CancellationToken cancellationToken = default);
    }
}
