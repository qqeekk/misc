using System.Collections.Generic;

namespace AntiSoft.SensoGraph.Infrastructure.Settings
{
    /// <summary>
    /// Global application settings.
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Metrics.
        /// </summary>
        public List<MetricSettings> Metrics { get; set; } = new();

        /// <summary>
        /// Interval of metrics write.
        /// </summary>
        public int IntervalSecs { get; set; } = 60;

        /// <summary>
        /// Enable metrics runner to collect metrics.
        /// </summary>
        public bool EnableMetricsRunner { get; set; } = true;

        /// <summary>
        /// Remove records older than certain amount of days.
        /// </summary>
        public int? DaysToKeepRecords { get; set; } = 90;
    }
}
