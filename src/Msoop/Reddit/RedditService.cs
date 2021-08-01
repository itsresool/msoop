using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Msoop.Reddit
{
    public class RedditService
    {
        private readonly HttpClient _apiClient;

        public RedditService(HttpClient apiClient,
            IOptions<RedditOptions> options)
        {
            _apiClient = apiClient;
            var opts = options.Value;

            _apiClient.BaseAddress = new Uri(opts.ApiBaseAddress);
            // Reddit requires from each app to have a unique user-agent that conforms to their syntax
            // Yet using .Add on that user-agent will fail with FormatException
            _apiClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", opts.WebUserAgent);
        }

        public async Task<RedditResource<RedditListing>> GetTopLinks(string subredditName, int postAgeLimitInDays)
        {
            var ageOption = postAgeLimitInDays switch
            {
                <= 1 => "day",
                <= 7 => "week",
                <= 31 => "month",
                <= 365 => "year",
                _ => "all",
            };

            var queryString = new Dictionary<string, string>()
            {
                {"t", ageOption},
                {"limit", "44"}
            };
            var requestUri = QueryHelpers.AddQueryString($"/r/{subredditName}/top", queryString);
            return await _apiClient.GetFromJsonAsync<RedditResource<RedditListing>>(requestUri);
        }
    }
}
