using System;
using System.Collections.Generic;

namespace AntiSoft.SensoGraph.Domain.Entities
{
    /// <summary>
    /// Chart metrics values for specific timestamp.
    /// </summary>
    public class ChartItem
    {
        public static IReadOnlyDictionary<ChartStep, TimeSpan> ChartStepTimes = new Dictionary<ChartStep, TimeSpan>
        {
            [ChartStep.Seconds1] = TimeSpan.FromSeconds(1),
            [ChartStep.Seconds15] = TimeSpan.FromSeconds(15),
            [ChartStep.Seconds30] = TimeSpan.FromSeconds(30),
            [ChartStep.Minutes1] = TimeSpan.FromMinutes(1),
            [ChartStep.Minutes15] = TimeSpan.FromMinutes(15),
            [ChartStep.Minutes30] = TimeSpan.FromMinutes(30),
            [ChartStep.Hours1] = TimeSpan.FromHours(1),
            [ChartStep.Hours2] = TimeSpan.FromHours(2),
            [ChartStep.Hours3] = TimeSpan.FromHours(3),
            [ChartStep.Hours4] = TimeSpan.FromHours(4),
            [ChartStep.Days1] = TimeSpan.FromDays(1)
        };

        /// <summary>
        /// Dictionary of codes and values.
        /// </summary>
        public IDictionary<string, decimal?> CodeValues { get; } =
            new Dictionary<string, decimal?>();

        /// <summary>
        /// Timestamp.
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
