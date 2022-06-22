using System;
using System.Collections.Generic;

namespace AntiSoft.SensoGraph.Models.Home
{
    /// <summary>
    /// Chart data model.
    /// </summary>
    public class ChartDataModel
    {
        /// <summary>
        /// Metric code.
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Chart label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// X axis label.
        /// </summary>
        public string XLabel { get; set; }

        /// <summary>
        /// Y axis label.
        /// </summary>
        public string YLabel { get; set; }

        /// <summary>
        /// Array of dates.
        /// </summary>
        public IList<DateTime> Dates { get; } = new List<DateTime>();

        /// <summary>
        /// Array of values for the dates.
        /// </summary>
        public IList<decimal?> Values { get; } = new List<decimal?>();

        /// <summary>
        /// Does it have any data.
        /// </summary>
        public bool HasData => Dates.Count > 0 && Values.Count > 0;

        /// <summary>
        /// The minimum value for the selected period.
        /// </summary>
        public decimal? MinimumValueForPeriod { get; set; }

        /// <summary>
        /// The maximum value for the selected period.
        /// </summary>
        public decimal? MaximumValueForPeriod { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="code">Metric code.</param>
        public ChartDataModel(string code)
        {
            this.Code = code;
        }
    }
}
