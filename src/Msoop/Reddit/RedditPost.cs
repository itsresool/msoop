using System;
using System.Text.Json.Serialization;

namespace Msoop.Reddit
{
    public class RedditPost
    {
        public string Id { get; set; }
        public string Subreddit { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }

        [JsonPropertyName("permalink")]
        [JsonConverter(typeof(DiscussionUrlConverter))]
        public string DiscussionUrl { get; set; }

        [JsonConverter(typeof(DateTimeOffsetConverter))]
        [JsonPropertyName("created_utc")]
        public DateTimeOffset CreatedUtc { get; set; }

        public int Score { get; set; }

        [JsonPropertyName("num_comments")]
        public int CommentsCount { get; set; }
    }
}
