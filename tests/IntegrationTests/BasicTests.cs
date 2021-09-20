using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Msoop.Web;
using Xunit;

namespace IntegrationTests
{
    public class BasicTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public BasicTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/Privacy")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType!.ToString());
        }

        [Fact]
        public async Task GivenWrongPath_ReturnCustomNotFoundPage()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("badpath");

            Assert.False(response.IsSuccessStatusCode);
            Assert.Contains("Page not found", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task EndpointsComplyWithGdpr()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/");
            var content = await response.Content.ReadAsStringAsync();

            Assert.Contains("This site uses cookies to track", content);
        }
    }
}
