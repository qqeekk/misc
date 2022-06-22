using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AntiSoft.SensoGraph.Domain.Abstractions;
using AntiSoft.SensoGraph.Domain.MetricProviders.Internal;

namespace AntiSoft.SensoGraph.Domain.MetricProviders
{
    /// <summary>
    /// Get the temperature value from TVK6.
    /// </summary>
    public class ExternalContentMetricProvider : IMetricProvider
    {
        private const string UriKey = "uri";
        private const string UnitOfMeasurementKey = "unitOfMeasurement";
        private const string RegexpKey = "regexp";
        private const string PointPlaceKey = "pointPlace";

        /// <inheritdoc />
        public string UnitOfMeasurement { get; private set; } = "\u00B0C";

        private string uri;

        private Regex regexp;

        private int pointPlace;

        private readonly ContentDownloader contentDownloader = new ();

        /// <inheritdoc />
        public Task Initialize(IDictionary<string, string> options)
        {
            if (!options.TryGetValue(UriKey, out uri))
            {
                throw new InvalidOperationException("The parameter \"uri\" is required.");
            }

            if (options.ContainsKey(UnitOfMeasurementKey))
            {
                UnitOfMeasurement = options[UnitOfMeasurementKey];
            }

            if (options.ContainsKey(RegexpKey))
            {
                regexp = new Regex(options[RegexpKey], RegexOptions.Compiled | RegexOptions.Multiline);
            }

            if (options.ContainsKey(PointPlaceKey))
            {
                pointPlace = int.Parse(options[PointPlaceKey]);
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task<decimal> GetValue(CancellationToken cancellationToken = default)
        {
            var body = await contentDownloader.LoadAsync(uri, cancellationToken);

            // Apply regexp.
            if (regexp != null)
            {
                var groups = regexp.Match(body);
                if (!groups.Success)
                {
                    throw new InvalidOperationException("Cannot get regexp group.");
                }
                body = groups.Groups.Values.Last().Value;
            }

            // Parse.
            var result = decimal.Parse(body);

            // Move point.
            if (pointPlace > 0)
            {
                result = result / (decimal)(Math.Pow(10, pointPlace));
            }
            return result;
        }
    }
}
