using System.Collections.Generic;
using System.Linq;

namespace AntiSoft.SensoGraph.Domain.Extensions
{
    /// <summary>
    /// Extensions for <see cref="IEnumerable{T}" />.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Calculate average value or return null.
        /// </summary>
        /// <param name="source">Source.</param>
        /// <returns>Average value or null.</returns>
        public static decimal? AverageOrNull(this IEnumerable<decimal> source) => source.Any() ? source.Average() : null;

        /// <summary>
        /// Calculate average value or return null.
        /// </summary>
        /// <param name="source">Source.</param>
        /// <returns>Average value or null.</returns>
        public static decimal? AverageOrNull(this IEnumerable<decimal?> source) => source.Any() ? source.Average() : null;
    }
}
