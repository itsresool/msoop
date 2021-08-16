using System.ComponentModel.DataAnnotations;

namespace Msoop.Core.Models
{
    public enum PostOrdering
    {
        Newest,
        Oldest,

        [Display(Name = "Score Descending")]
        ScoreDesc,

        [Display(Name = "Comments Descending")]
        CommentsDesc,
    }
}
