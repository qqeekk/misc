namespace CustomExpressionsDemo
{
    /// <summary>
    /// Sample marker with properties.
    /// </summary>
    public class Marker
    {
        public int MarkerNum { get; }

        public double D1 { get; set; }

        public double D2 { get; set; }

        public double D3 { get; set; }

        public Marker(int markerNum)
        {
            MarkerNum = markerNum;
        }
    }
}
