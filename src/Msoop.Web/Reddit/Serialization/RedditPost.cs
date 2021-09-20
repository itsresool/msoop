using System;
using System.Text.Json.Serialization;

namespace Msoop.Web.Reddit.Serialization
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

        public string Domain { get; set; }

        public bool IsFromReddit => IsSelf || IsRedditMediaDomain || Domain == "reddit.com";

        [JsonInclude]
        [JsonPropertyName("is_self")]
        public bool IsSelf { private get; set; }

        [JsonInclude]
        [JsonPropertyName("is_reddit_media_domain")]
        public bool IsRedditMediaDomain { private get; set; }

        [JsonPropertyName("over_18")]
        public bool IsOver18 { get; set; }

        [JsonPropertyName("spoiler")]
        public bool IsSpoiler { get; set; }

        [JsonPropertyName("is_stickied")]
        public bool IsStickied { get; set; }
    }
}
