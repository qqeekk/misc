using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AntiSoft.SensoGraph.Domain.Abstractions;
using AntiSoft.SensoGraph.Domain.Entities;
using AntiSoft.SensoGraph.Infrastructure.Settings;

namespace AntiSoft.SensoGraph.Infrastructure
{
    /// <summary>
    /// Hosted service to periodically call all metric providers.
    /// </summary>
    public sealed class MetricsExecutionHostedService : IHostedService, IDisposable
    {
        private static readonly TimeSpan MaintenancePeriod = TimeSpan.FromDays(1);

        private readonly MetricProvidersContainer metricProvidersContainer;
        private readonly IChartRepository chartRepository;
        private readonly IOptions<AppSettings> appSettings;
        private readonly ILogger<MetricsExecutionHostedService> logger;
        private Timer timer;
        private bool inProgress;
        private DateTime? lastMaintenanceDate;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="metricProvidersContainer">Metric providers container.</param>
        /// <param name="chartRepository">Chart repository.</param>
        /// <param name="appSettings">Application settings.</param>
        /// <param name="logger">Logger.</param>
        public MetricsExecutionHostedService(
            MetricProvidersContainer metricProvidersContainer,
            IChartRepository chartRepository,
            IOptions<AppSettings> appSettings,
            ILogger<MetricsExecutionHostedService> logger)
        {
            this.metricProvidersContainer = metricProvidersContainer;
            this.chartRepository = chartRepository;
            this.appSettings = appSettings;
            this.logger = logger;
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (!appSettings.Value.EnableMetricsRunner)
            {
                logger.LogInformation("Runner is disabled.");
                return Task.CompletedTask;
            }

            logger.LogInformation("MetricsExecutionHostedService running.");

            timer = new Timer(ExecuteTask, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(appSettings.Value.IntervalSecs));

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("MetricsExecutionHostedService stopping.");
            timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private async void ExecuteTask(object state)
        {
            if (inProgress)
            {
                return;
            }
            inProgress = true;

            try
            {
                await GetMetricsAndRecord();
                await MakeMaintenance();
            }
            catch (Exception ex)
            {
                logger.LogError("Error while getting values.", ex);
            }
            finally
            {
                inProgress = false;
            }
        }

        private async Task GetMetricsAndRecord()
        {
            var graphEntry = new ChartItem();
            // TODO: We can call GetValue method in multiple threads.
            foreach (var metric in metricProvidersContainer.GetAll())
            {
                try
                {
                    var value = await metric.Value.GetValue();
                    graphEntry.CodeValues[metric.Key] = value;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Cannot run {metric}: {message}.", metric.Value.GetType().Name, ex.Message);
                    graphEntry.CodeValues[metric.Key] = null;
                }
            }
            await chartRepository.Add(graphEntry);
        }

        private async Task MakeMaintenance()
        {
            if (!appSettings.Value.DaysToKeepRecords.HasValue)
            {
                return;
            }

            var now = DateTime.Now;
            if (!lastMaintenanceDate.HasValue || lastMaintenanceDate + MaintenancePeriod >= now)
            {
                try
                {
                    var removedCount = await chartRepository.RemoveOldRecords(
                        now.AddDays(-appSettings.Value.DaysToKeepRecords.Value));
                    logger.LogInformation("Maintenance done. Removed {removedCount} record(-s).", removedCount);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Cannot done maintenance.", ex.Message);
                }
                finally
                {
                    lastMaintenanceDate = now;
                }
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
