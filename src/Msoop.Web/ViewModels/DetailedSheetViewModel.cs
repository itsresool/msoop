using System.Collections.Generic;
using Msoop.Core.Models;
using Msoop.Web.Reddit.Serialization;

namespace Msoop.Web.ViewModels
{
    public class DetailedSheetViewModel
    {
        public IEnumerable<Subreddit> Subreddits { get; set; }

        public class Subreddit
        {
            public string Name { get; set; }
            public PostOrdering PostOrdering { get; set; }
            public IEnumerable<RedditPost> Posts { get; set; }
        }
    }
}
