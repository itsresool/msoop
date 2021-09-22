using System.Threading.Tasks;
using Msoop.Web.Reddit.Serialization;

namespace Msoop.Web.Reddit
{
    public interface IRedditService
    {
        Task<bool> SubredditExists(string name);
        Task<RedditResource<RedditListing>> GetTopListing(RedditListingCommand cmd);
    }

    public class RedditListingCommand
    {
        public string SubredditName { get; init; }
        public int MaxPostCount { get; init; }
        public int PostAgeLimitInDays { get; init; }
        public bool HasCustomPostAgeLimit => PostAgeLimitInDays is not 1 or 7 or 31 or 365;
    }
}
