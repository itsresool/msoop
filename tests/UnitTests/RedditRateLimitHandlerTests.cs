using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Msoop.Web.Reddit.Exceptions;
using Msoop.Web.Reddit.Handlers;
using Xunit;

namespace UnitTests
{
    public class RedditRateLimitHandlerTests
    {
        [Fact]
        public async Task ThrowsCustomException_WhenWeAreRateLimited()
        {
            var rateLimitHandler = new RateLimitHandler
            {
                InnerHandler = new TestingHandler(requestsLeft: 0),
            };
            var httpClient = new HttpClient(rateLimitHandler);

            // We have to run this at least once to get info about how many requests we have left
            await httpClient.GetAsync("http://test.com");

            await Assert.ThrowsAsync<RateLimitedException>(() => httpClient.GetAsync("http://test.com"));
        }

        // This handler returns response message that tells how many request we have left
        private class TestingHandler : DelegatingHandler
        {
            private readonly int _requestsLeft;

            public TestingHandler(int requestsLeft)
            {
                _requestsLeft = requestsLeft;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    RequestMessage = request,
                };
                response.Headers.Add("X-Ratelimit-Remaining", _requestsLeft.ToString());

                return Task.FromResult(response);
            }
        }
    }
}
