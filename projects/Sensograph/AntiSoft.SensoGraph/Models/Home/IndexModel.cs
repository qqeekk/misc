using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AntiSoft.SensoGraph.Domain.Entities;

namespace AntiSoft.SensoGraph.Models.Home
{
    /// <summary>
    /// Main model.
    /// </summary>
    public class IndexModel
    {
        /// <summary>
        /// Start date.
        /// </summary>
        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Start")]
        public DateTime Start { get; set; } = DateTime.Today.AddDays(-1);

        /// <summary>
        /// End date.
        /// </summary>
        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "End")]
        public DateTime End { get; set; } = DateTime.Today;

        /// <summary>
        /// Chart step.
        /// </summary>
        [Required]
        [EnumDataType(typeof(ChartStep))]
        public ChartStep Step { get; set; } = ChartStep.Minutes15;

        /// <summary>
        /// Charts to display.
        /// </summary>
        public List<ChartDataModel> Charts { get; } = new();
    }
}
