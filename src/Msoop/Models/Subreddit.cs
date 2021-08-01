using System;

namespace Msoop.Models
{
    public class Subreddit
    {
        public string Name { get; set; }
        public int MaxPostCount { get; set; }
        public PostOrdering PostOrdering { get; set; }

        public Guid SheetId { get; set; }
        public Sheet Sheet { get; set; }
    }
}
