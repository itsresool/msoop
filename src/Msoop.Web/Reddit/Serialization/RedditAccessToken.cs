using System;
using System.Text.Json.Serialization;

namespace Msoop.Web.Reddit.Serialization
{
    public class RedditAccessToken
    {
        private int _durationInSeconds;
        private DateTimeOffset _expiresAt;

        [JsonPropertyName("access_token")]
        public string Value { get; set; }

        [JsonPropertyName("token_type")]
        public string Type { get; set; }

        [JsonPropertyName("expires_in")]
        public int DurationInSeconds
        {
            get => _durationInSeconds;
            set
            {
                _durationInSeconds = value;
                _expiresAt = DateTimeOffset.Now.AddSeconds(_durationInSeconds);
            }
        }

        public bool IsExpired => _expiresAt <= DateTimeOffset.Now;

        public string Scope { get; set; }
    }
}
