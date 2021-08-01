namespace Msoop.Reddit
{
    public class RedditOptions
    {
        public const string Reddit = "Reddit";

        public string AuthorizationAddress { get; set; }
        public string ApiBaseAddress { get; set; }
        public string WebClientId { get; set; }
        public string WebSecret { get; set; }
        public string WebUserAgent { get; set; }
        public string WebRedirectUri { get; set; }
    }
}
