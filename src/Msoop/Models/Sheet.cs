using System;
using System.Collections.Generic;

namespace Msoop.Models
{
    public class Sheet
    {
        public Guid Id { get; set; }
        public int PostAgeLimitInDays { get; set; }
        public bool AllowOver18 { get; set; }
        public bool AllowSpoilers { get; set; }
        public bool AllowStickied { get; set; }
        
        public List<Subreddit> Subreddits { get; set; }
    }
}
