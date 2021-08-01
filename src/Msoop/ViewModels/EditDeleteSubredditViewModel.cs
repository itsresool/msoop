using System.ComponentModel.DataAnnotations;
using Msoop.Models;

namespace Msoop.ViewModels
{
    public class EditDeleteSubredditViewModel
    {
        public string Name { get; set; }

        [Display(Name = "How many posts to show")]
        [Range(1, 100, ErrorMessage = "Value must be between {1} and {2}")]
        public int MaxPostCount { get; set; } = 15;

        [Display(Name = "Order posts by")]
        public PostOrdering PostOrdering { get; set; } = PostOrdering.Newest;
    }
}
