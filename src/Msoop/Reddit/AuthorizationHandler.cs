using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Msoop.Reddit.Exceptions;

namespace Msoop.Reddit
{
    public class AuthorizationHandler : DelegatingHandler
    {
        private readonly HttpClient _httpClient;
        private RedditAccessToken _accessToken;
        private readonly SemaphoreSlim _fetchTokenSemaphore = new(1, 1);

        public AuthorizationHandler(HttpClient httpClient, IOptions<RedditOptions> options)
        {
            _httpClient = httpClient;
            var opts = options.Value;

            _httpClient.BaseAddress = new Uri(opts.AuthorizationAddress);
            _httpClient.DefaultRequestHeaders.Add("Authorization",
                GetBasicAuthValue(opts.WebClientId, opts.WebSecret));
            // Reddit requires from each app to have a unique user-agent that conforms to their syntax
            // Yet using .Add on that user-agent will fail with FormatException
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", opts.WebUserAgent);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (!request.Headers.Contains("Authorization"))
            {
                var token = await FetchAccessToken(cancellationToken);
                request.Headers.Add("Authorization", $"Bearer {token.Value}");
            }

            return await base.SendAsync(request, cancellationToken);
        }

        private string GetBasicAuthValue(string username, string password)
        {
            var encodedBase64Value = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
            return $"Basic {encodedBase64Value}";
        }

        private async Task<RedditAccessToken> FetchAccessToken(CancellationToken cancellationToken)
        {
            await _fetchTokenSemaphore.WaitAsync(cancellationToken);
            try
            {
                if (_accessToken == null || _accessToken.IsExpired)
                {
                    var dict = new Dictionary<string, string> {{"grant_type", "client_credentials"}};
                    var resp = await _httpClient.PostAsync("/api/v1/access_token", new FormUrlEncodedContent(dict),
                        cancellationToken);
                    _accessToken =
                        await resp.Content.ReadFromJsonAsync<RedditAccessToken>(cancellationToken: cancellationToken);
                }
            }
            catch (Exception)
            {
                throw new RedditServiceException("Failed to authorize with reddit services.");
            }
            finally
            {
                _fetchTokenSemaphore.Release();
            }

            return _accessToken;
        }
    }
}
