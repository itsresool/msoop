using System.Collections.Generic;
using Msoop.Models;
using Msoop.Reddit;

namespace Msoop.ViewModels
{
    public class SubredditViewModel
    {
        public string Name { get; set; }
        public PostOrdering PostOrdering { get; set; }
        public IEnumerable<RedditPost> Posts { get; set; }
    }
}
