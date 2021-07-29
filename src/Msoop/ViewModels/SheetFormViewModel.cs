using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Msoop.ViewModels
{
    public class SheetFormViewModel
    {
        public Guid Id { get; set; }
        public PostAgeLimit PostAgeLimit { get; set; }

        [Range(1, 9999, ErrorMessage = "Value must be between {1} and {2}")]
        public int CustomAgeLimit { get; set; }

        public IList<CreateSubredditViewModel> Subreddits { get; set; }
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
