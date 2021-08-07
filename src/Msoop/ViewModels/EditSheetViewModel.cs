using System.ComponentModel.DataAnnotations;

namespace Msoop.ViewModels
{
    public class EditSheetViewModel
    {
        public PostAgeLimit PostAgeLimit { get; set; }

        [Range(1, 9999, ErrorMessage = "Value must be between {1} and {2}")]
        public int CustomAgeLimit { get; set; }

        [Display(Name = "Allow posts that contain adult content")]
        public bool AllowOver18 { get; set; }

        [Display(Name = "Allow posts that contain spoilers")]
        public bool AllowSpoilers { get; set; }

        [Display(Name = "Allow stickied posts")]
        public bool AllowStickied { get; set; }
    }

    public enum PostAgeLimit
    {
        [Display(Name = "Last Day")]
        LastDay,

        [Display(Name = "Last Week")]
        LastWeek,

        [Display(Name = "Last Month")]
        LastMonth,

        [Display(Name = "Last Year")]
        LastYear,

        [Display(Name = "Custom Period")]
        Custom
    }
}
