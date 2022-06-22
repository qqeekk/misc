using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AntiSoft.SensoGraph.Domain.MetricProviders.Internal
{
    /// <summary>
    /// The class allows to download content depends on protocol. Only HTTP, HTTPS and local files are supported.
    /// </summary>
    internal class ContentDownloader
    {
        /// <summary>
        /// Load content by string URI.
        /// </summary>
        /// <param name="uri">URI.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Content.</returns>
        public Task<string> LoadAsync(string uri, CancellationToken cancellationToken)
        {
            if (uri.StartsWith(@"http://", StringComparison.OrdinalIgnoreCase) ||
                uri.StartsWith(@"https://", StringComparison.OrdinalIgnoreCase))
            {
                return LoadHttp(uri, cancellationToken);
            }
            // Fallback.
            return LoadLocal(uri, cancellationToken);
        }

        private static async Task<string> LoadHttp(string uri, CancellationToken cancellationToken)
        {
            var webRequest = WebRequest.CreateHttp(uri);
            var response = webRequest.GetResponse();
            await using var stream = response.GetResponseStream();
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();
            response.Close();
            return content;
        }

        private static Task<string> LoadLocal(string uri, CancellationToken cancellationToken) =>
            File.ReadAllTextAsync(uri, cancellationToken);
    }
}
