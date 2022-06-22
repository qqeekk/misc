using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AntiSoft.SensoGraph.Domain.Abstractions;
using AntiSoft.SensoGraph.Domain.Entities;
using AntiSoft.SensoGraph.Domain.Extensions;
using AntiSoft.SensoGraph.Infrastructure.Settings;
using AntiSoft.SensoGraph.Models.Home;

namespace AntiSoft.SensoGraph.Controllers
{
    /// <summary>
    /// Index controller.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly IChartRepository chartRepository;
        private readonly MetricProvidersContainer metricProvidersContainer;
        private readonly IOptions<AppSettings> settings;

        public HomeController(
            IChartRepository chartRepository,
            MetricProvidersContainer metricProvidersContainer,
            IOptions<AppSettings> settings)
        {
            this.chartRepository = chartRepository;
            this.metricProvidersContainer = metricProvidersContainer;
            this.settings = settings;
        }

        [HttpGet]
        public async Task<IActionResult> Index(IndexModel model, CancellationToken cancellationToken)
        {
            var step = ChartItem.ChartStepTimes[model.Step];
            model.End = model.End.Add(step);
            var entries = await chartRepository.GetForPeriod(model.Start, model.End, cancellationToken);
            model.Charts.AddRange(GetChartData(entries, model.Start, model.End, step));

            return View(model);
        }

        private IList<ChartDataModel> GetChartData(IList<ChartItem> entries, DateTime start, DateTime end, TimeSpan step)
        {
            return settings.Value.Metrics
                .Where(m => m.Enabled)
                .Select(m => m.Code)
                .Select(code => GetChartDataByCode(entries, code, start, end, step))
                .Where(chartData => chartData.HasData)
                .Select(FillLabels)
                .Select(FillMinAndMaxValues)
                .ToList();
        }

        private ChartDataModel FillLabels(ChartDataModel model)
        {
            var metric = settings.Value.Metrics.FirstOrDefault(m => m.Code == model.Code);
            if (metric != null)
            {
                model.Label = metric.Label;
                model.XLabel = Resources.Shared.Dates;
                model.YLabel = metricProvidersContainer.GetByCode(metric.Code).UnitOfMeasurement;
            }
            return model;
        }

        private ChartDataModel FillMinAndMaxValues(ChartDataModel model)
        {
            if (model.HasData)
            {
                model.MinimumValueForPeriod = model.Values.Min();
                model.MaximumValueForPeriod = model.Values.Max();
            }
            return model;
        }

        private static ChartDataModel GetChartDataByCode(IList<ChartItem> entries, string code,
            DateTime start, DateTime end, TimeSpan step)
        {
            var chartData = new ChartDataModel(code);
            foreach (var intervalsForDate in DateTimeUtils.GetIntervalsForDates(start, end, step))
            {
                var intervalAvgValue = entries
                    .Where(d => d.Timestamp >= intervalsForDate.intervalStart && d.Timestamp <= intervalsForDate.intervalEnd &&
                        d.CodeValues.ContainsKey(code))
                    .Select(d => d.CodeValues[code])
                    .AverageOrNull();
                chartData.Dates.Add(intervalsForDate.intervalStart);
                chartData.Values.Add(intervalAvgValue.HasValue ? Math.Round(intervalAvgValue.Value, 2) : null);
            }
            return chartData;
        }
    }
}
