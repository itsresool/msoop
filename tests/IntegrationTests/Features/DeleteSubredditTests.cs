using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Msoop.Web.Features.Subreddits;
using Xunit;

namespace IntegrationTests.Features
{
    [Collection("MsoopTests")]
    public class DeleteSubredditTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;

        public DeleteSubredditTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Handler_ThrowsInvalidOperationException_WhenGivenSheetDoesNotHaveThatSubreddit()
        {
            await using var handlerContext = _fixture.CreateContext();
            await using var testingContext = _fixture.CreateContext();
            var sheet = await testingContext.Sheets.FirstAsync();
            var cmd = new DeleteSubreddit.Command { SheetId = sheet.Id, SubName = "test" };
            var handler = new DeleteSubreddit.Handler(handlerContext);

            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(cmd, CancellationToken.None));
        }
    }
}
