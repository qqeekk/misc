using System.Linq;
using System.Threading.Tasks;
using Extensions.Hosting.AsyncInitialization;
using Microsoft.Extensions.Options;
using AntiSoft.SensoGraph.Domain.Abstractions;
using AntiSoft.SensoGraph.Infrastructure.Settings;

namespace AntiSoft.SensoGraph.Infrastructure.Startup
{
    /// <summary>
    /// Contains database migration helper methods.
    /// </summary>
    internal sealed class DatabaseInitializer : IAsyncInitializer
    {
        private readonly IChartRepository chartRepository;
        private readonly IOptions<AppSettings> settings;

        /// <summary>
        /// Database initializer. Performs migration and data seed.
        /// </summary>
        /// <param name="chartRepository">Data repository.</param>
        /// <param name="settings">Application settings.</param>
        public DatabaseInitializer(IChartRepository chartRepository, IOptions<AppSettings> settings)
        {
            this.chartRepository = chartRepository;
            this.settings = settings;
        }

        /// <inheritdoc />
        public async Task InitializeAsync()
        {
            await chartRepository.Initialize(
                settings.Value.Metrics.Select(m => m.Code).ToArray());
        }
    }
}
