using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Msoop.Web.Reddit.Serialization
{
    public class RedditListing
    {
        public string Before { get; set; }
        public string After { get; set; }

        [JsonPropertyName("dist")]
        public int Count { get; set; }

        public IEnumerable<RedditResource<RedditPost>> Children { get; set; }
    }
}
