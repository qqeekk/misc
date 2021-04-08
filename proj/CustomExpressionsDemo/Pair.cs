using System.Collections.Generic;

namespace CustomExpressionsDemo
{
    /// <summary>
    /// Pair is the struct of two markers.
    /// </summary>
    public class Pair
    {
        /// <summary>
        /// Pair number.
        /// </summary>
        public int PairNum { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Marker 1 number.
        /// </summary>
        public int MarkerNum1 { get; set; }

        /// <summary>
        /// Marker 1.
        /// </summary>
        public IDictionary<string, object> Start { get; set; }

        /// <summary>
        /// Marker 2 number.
        /// </summary>
        public int MarkerNum2 { get; set; }

        /// <summary>
        /// Marker 2.
        /// </summary>
        public IDictionary<string, object> End { get; set; }
    }
}
