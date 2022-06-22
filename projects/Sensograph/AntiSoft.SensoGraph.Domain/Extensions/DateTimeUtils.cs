using System;
using System.Collections.Generic;

namespace AntiSoft.SensoGraph.Domain.Extensions
{
    /// <summary>
    /// Utilities for <see cref="DateTime" />.
    /// </summary>
    public static class DateTimeUtils
    {
        public static IEnumerable<(DateTime intervalStart, DateTime intervalEnd)> GetIntervalsForDates(
            DateTime start, DateTime end, TimeSpan step)
        {
            var currentDate = start;
            while (currentDate < end)
            {
                var currentDateEnd = currentDate.Add(step);
                yield return (currentDate, currentDateEnd);
                currentDate = currentDateEnd;
            }
        }
    }
}
