using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Contrib.HttpClient;
using Msoop.Web.Reddit;
using Msoop.Web.Reddit.Handlers;
using Xunit;

namespace UnitTests
{
    public class RedditAuthorizationHandlerTests
    {
        private const string AuthResponse = "{\"expires_in\": 3600, \"access_token\": \"foo\"}";
        private const string ExpiredAuthResponse = "{\"expires_in\": 0, \"access_token\": \"foo\"}";

        private readonly IOptions<RedditOptions> _fakeOptions = Options.Create(new RedditOptions
        {
            AuthorizationBaseAddress = "http://test.com",
            WebSecret = "",
            WebClientId = "",
            WebUserAgent = "",
        });

        [Fact]
        public async Task FillsAuthorizationHeader_WhenItIsEmpty()
        {
            var mockAuthHttp = new Mock<HttpMessageHandler>();
            mockAuthHttp.SetupAnyRequest().ReturnsResponse(AuthResponse);
            var authHandler = new AuthorizationHandler(mockAuthHttp.CreateClient(), _fakeOptions)
            {
                InnerHandler = new TestingHandler(),
            };
            var httpClient = new HttpClient(authHandler);

            var response = await httpClient.GetAsync("http://test.com");

            Assert.False(httpClient.DefaultRequestHeaders.Contains("Authorization"));
            Assert.True(response.RequestMessage!.Headers.Contains("Authorization"));
        }

        [Fact]
        public async Task DoesNotRefreshAccessToken_WhenItIsNotExpired()
        {
            var mockAuthHttp = new Mock<HttpMessageHandler>();
            mockAuthHttp.SetupAnyRequest().ReturnsResponse(AuthResponse);
            var authHandler = new AuthorizationHandler(mockAuthHttp.CreateClient(), _fakeOptions)
            {
                InnerHandler = new TestingHandler(),
            };
            var httpClient = new HttpClient(authHandler);

            await httpClient.GetAsync("http://test.com");
            // If this throws it means the token was expired
            mockAuthHttp.SetupAnyRequest().Throws<NotImplementedException>();

            var exception = await Record.ExceptionAsync(() => httpClient.GetAsync("http://test.com"));
            Assert.Null(exception);
        }

        [Fact]
        public async Task RefreshesAccessToken_WhenItIsExpired()
        {
            var mockAuthHttp = new Mock<HttpMessageHandler>();
            mockAuthHttp.SetupAnyRequest().ReturnsResponse(ExpiredAuthResponse);
            var authHandler = new AuthorizationHandler(mockAuthHttp.CreateClient(), _fakeOptions)
            {
                InnerHandler = new TestingHandler(),
            };
            var httpClient = new HttpClient(authHandler);

            await httpClient.GetAsync("http://test.com");
            // If this does not throw it means the token is still fresh
            mockAuthHttp.SetupAnyRequest().Throws<NotImplementedException>();

            // We are using .ThrowsAnyAsync, because authHandler rethrows with its own exception
            await Assert.ThrowsAnyAsync<Exception>(() => httpClient.GetAsync("http://test.com"));
        }

        private class TestingHandler : DelegatingHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    RequestMessage = request,
                });
            }
        }
    }
}
