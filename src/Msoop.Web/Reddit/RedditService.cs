using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Msoop.Web.Reddit.Exceptions;
using Msoop.Web.Reddit.Serialization;

namespace Msoop.Web.Reddit
{
    public class RedditService : IRedditService
    {
        private const int MaxListingLimit = 100;
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

        public async Task<bool> SubredditExists(string name)
        {
            try
            {
                var resp = await _apiClient.GetAsync($"/r/{name}/about");
                return resp.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                throw new RedditServiceException($"Failed to check if /r/{name} exists.");
            }
        }

        public async Task<RedditResource<RedditListing>> GetTopListing(RedditListingCommand cmd)
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

            var queryString = new Dictionary<string, string>
            {
                { "t", ageOption },
                { "limit", listingLimit.ToString() },
            };
            var requestUri = QueryHelpers.AddQueryString($"/r/{cmd.SubredditName}/top", queryString);
            try
            {
                return await _apiClient.GetFromJsonAsync<RedditResource<RedditListing>>(requestUri);
            }
            catch (Exception)
            {
                throw new RedditServiceException($"Failed to fetch /r/{cmd.SubredditName} listing.");
            }
        }
    }
}
