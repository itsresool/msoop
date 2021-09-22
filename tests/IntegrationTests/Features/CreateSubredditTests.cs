using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Msoop.Web.Features.Subreddits;
using Msoop.Web.Reddit;
using Msoop.Web.ViewModels;
using Xunit;

namespace IntegrationTests.Features
{
    [Collection("MsoopTests")]
    public class CreateSubredditTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;

        public CreateSubredditTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Handler_ReturnsSubredditNotFound_WhenSubredditNameDoesNotExistsInReddit()
        {
            await using var handlerContext = _fixture.CreateContext();
            var mockRedditService = new Mock<IRedditService>();
            mockRedditService.Setup(m => m.SubredditExists(It.IsAny<string>()))
                .Returns(Task.FromResult(result: false));
            var cmd = new CreateSubreddit.Command(new Guid(), new CreateSubredditViewModel { Name = "" });
            var handler = new CreateSubreddit.Handler(handlerContext, mockRedditService.Object);

            var actual = await handler.Handle(cmd, CancellationToken.None);

            Assert.Equal(CreateSubreddit.Response.SubredditNotFound, actual);
        }

        [Fact]
        public async Task Handler_ReturnsSubredditAlreadyAddedResponse_WhenSubredditExistsInDb()
        {
            await using var handlerContext = _fixture.CreateContext();
            await using var testingContext = _fixture.CreateContext();
            var subreddit = await testingContext.Subreddits.FirstAsync(s => s.Name == "dotnet");
            var mockRedditService = new Mock<IRedditService>();
            mockRedditService.Setup(m => m.SubredditExists(It.IsAny<string>()))
                .Returns(Task.FromResult(result: true));
            var cmd = new CreateSubreddit.Command(subreddit.SheetId, new CreateSubredditViewModel { Name = "dotnet" });
            var handler = new CreateSubreddit.Handler(handlerContext, mockRedditService.Object);

            var actual = await handler.Handle(cmd, CancellationToken.None);

            Assert.Equal(CreateSubreddit.Response.SubredditAlreadyAdded, actual);
        }

        [Fact]
        public async Task Handler_ReturnsOkResponse_WhenInputIsValid()
        {
            await using var handlerContext = _fixture.CreateContext();
            await using var testingContext = _fixture.CreateContext();
            var subreddit = await testingContext.Subreddits.FirstAsync(s => s.Name == "dotnet");
            var mockRedditService = new Mock<IRedditService>();
            mockRedditService.Setup(m => m.SubredditExists(It.IsAny<string>()))
                .Returns(Task.FromResult(result: true));
            var cmd = new CreateSubreddit.Command(subreddit.SheetId, new CreateSubredditViewModel { Name = "test" });
            var handler = new CreateSubreddit.Handler(handlerContext, mockRedditService.Object);

            var actual = await handler.Handle(cmd, CancellationToken.None);

            Assert.Equal(CreateSubreddit.Response.Ok, actual);
        }
    }
}
