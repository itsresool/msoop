using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Msoop.Reddit
{
    public class RedditService
    {
        private readonly HttpClient _httpClient;

        public RedditService(HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri("https://www.reddit.com");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Msoop");
            _httpClient = httpClient;
        }

        // /r/${subredditName}/top/.json?sort=top&t=${timePeriod}&before=${before}&after=${after}&count=${count}&limit=${limit}&raw_json=1
        public async Task<RedditResource<RedditListing>> GetTopLinks(string subredditName, int postAge)
        {
            var ageOption = postAge switch
            {
                <= 1 => "day",
                <= 7 => "week",
                <= 31 => "month",
                <= 365 => "year",
                _ => "all",
            };
            return await _httpClient.GetFromJsonAsync<RedditResource<RedditListing>>(
                $"/r/{subredditName}/top/.json?sort=top&t={ageOption}&raw_json=1");
        }
    }
}
