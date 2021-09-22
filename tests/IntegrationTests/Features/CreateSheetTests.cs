using System;
using System.Threading;
using System.Threading.Tasks;
using Msoop.Web.Features.Sheets;
using Xunit;

namespace IntegrationTests.Features
{
    [Collection("MsoopTests")]
    public class CreateSheetTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;

        public CreateSheetTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }


        [Fact]
        public async Task Handler_ReturnsNewSheetId()
        {
            await using var handlerContext = _fixture.CreateContext();
            await using var testingContext = _fixture.CreateContext();
            var cmd = new CreateSheet.Command();
            var handler = new CreateSheet.Handler(handlerContext);

            var sheetId = await handler.Handle(cmd, CancellationToken.None);
            var item = await testingContext.Sheets.FindAsync(sheetId);

            Assert.IsType<Guid>(sheetId);
            Assert.NotNull(item);
        }

        [Fact]
        public async Task Handler_NewSheet_AllowsForNsfwAndSpoilerLinks()
        {
            await using var handlerContext = _fixture.CreateContext();
            await using var testingContext = _fixture.CreateContext();
            var cmd = new CreateSheet.Command();
            var handler = new CreateSheet.Handler(handlerContext);

            var sheetId = await handler.Handle(cmd, CancellationToken.None);
            var item = await testingContext.Sheets.FindAsync(sheetId);

            Assert.True(item.AllowOver18);
            Assert.True(item.AllowSpoilers);
        }

        [Fact]
        public async Task Handler_NewSheet_DisallowsStickyLinks()
        {
            await using var handlerContext = _fixture.CreateContext();
            await using var testingContext = _fixture.CreateContext();
            var cmd = new CreateSheet.Command();
            var handler = new CreateSheet.Handler(handlerContext);

            var sheetId = await handler.Handle(cmd, CancellationToken.None);
            var item = await testingContext.Sheets.FindAsync(sheetId);

            Assert.False(item.AllowStickied);
        }

        [Fact]
        public async Task Handler_NewSheet_DefaultsTo7DayPostAgeLimit()
        {
            await using var handlerContext = _fixture.CreateContext();
            await using var testingContext = _fixture.CreateContext();
            var cmd = new CreateSheet.Command();
            var handler = new CreateSheet.Handler(handlerContext);

            var sheetId = await handler.Handle(cmd, CancellationToken.None);
            var item = await testingContext.Sheets.FindAsync(sheetId);

            Assert.Equal(expected: 7, item.PostAgeLimitInDays);
        }
    }
}
