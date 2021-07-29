using System.ComponentModel.DataAnnotations;

namespace Msoop.Models
{
    public enum PostOrdering
    {
        Newest,
        Oldest,

        [Display(Name = "Score Descending")]
        ScoreDesc,

        [Display(Name = "Comments Descending")]
        CommentsDesc
    }
}
