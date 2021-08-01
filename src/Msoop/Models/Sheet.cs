using System;
using System.Collections.Generic;

namespace Msoop.Models
{
    public class Sheet
    {
        public Guid Id { get; set; }
        public int PostAgeLimitInDays { get; set; }
        public List<Subreddit> Subreddits { get; set; }
    }
}
