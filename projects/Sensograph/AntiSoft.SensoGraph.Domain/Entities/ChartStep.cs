using System.ComponentModel.DataAnnotations;

namespace AntiSoft.SensoGraph.Domain.Entities
{
    /// <summary>
    /// Chart steps.
    /// </summary>
    public enum ChartStep
    {
        [Display(Name = "1 sec")]
        Seconds1,

        [Display(Name = "15 secs")]
        Seconds15,

        [Display(Name = "30 secs")]
        Seconds30,

        [Display(Name = "1 min")]
        Minutes1,

        [Display(Name = "15 mins")]
        Minutes15,

        [Display(Name = "30 mins")]
        Minutes30,

        [Display(Name = "1 hour")]
        Hours1,

        [Display(Name = "2 hours")]
        Hours2,

        [Display(Name = "3 hours")]
        Hours3,

        [Display(Name = "4 hours")]
        Hours4,

        [Display(Name = "1 day")]
        Days1
    }
}
