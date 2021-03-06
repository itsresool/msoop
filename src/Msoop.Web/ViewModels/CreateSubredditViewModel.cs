using System.ComponentModel.DataAnnotations;
using Msoop.Core.Models;

namespace Msoop.Web.ViewModels
{
    public class CreateSubredditViewModel
    {
        [Required]
        public string Name { get; set; }

        [Display(Name = "How many posts to show")]
        [Range(minimum: 1, maximum: 100, ErrorMessage = "Value must be between {1} and {2}")]
        public int MaxPostCount { get; set; } = 15;

        [Display(Name = "Order posts by")]
        public PostOrdering PostOrdering { get; set; } = PostOrdering.Newest;
    }
}
