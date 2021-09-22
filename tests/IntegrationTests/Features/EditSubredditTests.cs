using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Msoop.Web.Features.Subreddits;
using Msoop.Web.ViewModels;
using Xunit;

namespace IntegrationTests.Features
{
    [Collection("MsoopTests")]
    public class EditSubredditTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;

        public EditSubredditTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task CommandHandler_ThrowsInvalidOperationException_WhenGivenSheetDoesNotHaveThatSubreddit()
        {
            await using var handlerContext = _fixture.CreateContext();
            await using var testingContext = _fixture.CreateContext();
            var sheet = await testingContext.Sheets.FirstAsync();
            var query = new EditSubreddit.Query { SheetId = sheet.Id, SubName = "test" };
            var editForm = new EditSubredditViewModel();
            var cmd = new EditSubreddit.Command(query, editForm);
            var handler = new EditSubreddit.CommandHandler(handlerContext);

            await Assert.ThrowsAsync<InvalidOperationException>(()
                => handler.Handle(cmd, CancellationToken.None));
        }

        [Fact]
        public async Task CommandHandler_UpdatesSubreddit_GivenValidInput()
        {
            await using var handlerContext = _fixture.CreateContext();
            await using var prepareTestContext = _fixture.CreateContext();
            var subreddit = await prepareTestContext.Subreddits.FirstAsync();
            var query = new EditSubreddit.Query { SheetId = subreddit.SheetId, SubName = subreddit.Name };
            var editForm = new EditSubredditViewModel { MaxPostCount = 99 };
            var cmd = new EditSubreddit.Command(query, editForm);
            var handler = new EditSubreddit.CommandHandler(handlerContext);

            await handler.Handle(cmd, CancellationToken.None);
            await using var assertTestContext = _fixture.CreateContext();
            var actual =
                await assertTestContext.Subreddits.FirstAsync(s =>
                    s.SheetId == subreddit.SheetId && s.Name == subreddit.Name);

            Assert.NotEqual(subreddit.MaxPostCount, actual.MaxPostCount);
            Assert.Equal(expected: 99, actual.MaxPostCount);
        }
    }
}
