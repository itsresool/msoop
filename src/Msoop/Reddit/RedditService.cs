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
        public const int MaxListingLimit = 100;

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

        public async Task<bool> SubredditExists(string name)
        {
            var resp = await _apiClient.GetAsync($"/r/{name}/about");
            return resp.IsSuccessStatusCode;
        }

        public class ListingCommand
        {
            public string SubredditName { get; init; }
            public int MaxPostCount { get; init; }
            public int PostAgeLimitInDays { get; init; }
            public bool HasCustomPostAgeLimit => PostAgeLimitInDays is not 1 or 7 or 31 or 365;
        }

        public async Task<RedditResource<RedditListing>> GetTopListing(ListingCommand cmd)
        {
            var ageOption = cmd.PostAgeLimitInDays switch
            {
                <= 1 => "day",
                <= 7 => "week",
                <= 31 => "month",
                <= 365 => "year",
                _ => "all",
            };
            // We fetch maximum amount of posts that's allowed, when we use a custom age limit,
            // because for these we have to fetch a larger slice of data that contains both posts
            // belonging to our limit and those outside it.
            // Example: We want 20 posts from 15 days ago, so we fetch a listing from a month ago,
            // if we were to limit ourselves to 20 items, half of them would be outside our time period.
            var listingLimit = cmd.HasCustomPostAgeLimit ? MaxListingLimit : cmd.MaxPostCount;

            var queryString = new Dictionary<string, string>()
            {
                {"t", ageOption},
                {"limit", listingLimit.ToString()}
            };
            var requestUri = QueryHelpers.AddQueryString($"/r/{cmd.SubredditName}/top", queryString);
            return await _apiClient.GetFromJsonAsync<RedditResource<RedditListing>>(requestUri);
        }
    }
}
