using System.Collections.Generic;

namespace AntiSoft.SensoGraph.Infrastructure.Settings
{
    /// <summary>
    /// Metric settings.
    /// </summary>
    public class MetricSettings
    {
        /// <summary>
        /// Metric code. It will be used as database column name.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// The metric label to be shown on the graph.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// The type name of the metric provider. For example ExternalContentMetricProvider.
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// Is the metric enabled.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Metric options.
        /// </summary>
        public IDictionary<string, string> Options { get; set; } = new Dictionary<string, string>();
    }
}
