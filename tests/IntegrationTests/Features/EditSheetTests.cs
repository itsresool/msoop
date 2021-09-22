using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using Msoop.Core.Models;
using Msoop.Web.Features.Sheets;
using Msoop.Web.ViewModels;
using Xunit;

namespace IntegrationTests.Features
{
    [Collection("MsoopTests")]
    public class EditSheetTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;

        public EditSheetTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task QueryHandler_ReturnsNull_WhenSheetIdIsInvalid()
        {
            await using var handlerContext = _fixture.CreateContext();
            var cmd = new EditSheet.Query { Id = new Guid() };
            var mockMapper = new Mock<IMapper>();
            var handler = new EditSheet.QueryHandler(handlerContext, mockMapper.Object);

            var actual = await handler.Handle(cmd, CancellationToken.None);

            Assert.Null(actual);
        }

        [Fact]
        public async Task QueryHandler_ReturnsSheetWithItsSubreddits()
        {
            await using var handlerContext = _fixture.CreateContext();
            await using var testingContext = _fixture.CreateContext();
            var subreddit = await testingContext.Subreddits.FirstAsync();
            var sheet = await testingContext.Sheets.Include(s => s.Subreddits)
                .Where(s => s.Id == subreddit.SheetId)
                .FirstAsync();
            var cmd = new EditSheet.Query { Id = sheet.Id };
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Sheet, EditSheetViewModel>();
                cfg.CreateMap<Subreddit, SubredditSummaryViewModel>();
            });
            var handler = new EditSheet.QueryHandler(handlerContext, mapperConfiguration.CreateMapper());

            var actual = await handler.Handle(cmd, CancellationToken.None);

            Assert.Equal(sheet.Id, actual.Sheet.Id);
            Assert.Equal(sheet.Subreddits.Count, actual.OwnedSubreddits.Count);
        }

        [Fact]
        public async Task CommandHandler_ThrowsInvalidOperationException_WhenSheetIdIsInvalid()
        {
            await using var handlerContext = _fixture.CreateContext();
            await using var testingContext = _fixture.CreateContext();
            var cmd = new EditSheet.Command(new Guid(), new EditSheetViewModel());
            var handler = new EditSheet.CommandHandler(handlerContext);

            await Assert.ThrowsAsync<InvalidOperationException>(()
                => handler.Handle(cmd, CancellationToken.None));
        }

        [Fact]
        public async Task CommandHandler_UpdatesSheet()
        {
            await using var handlerContext = _fixture.CreateContext();
            await using var testingContext = _fixture.CreateContext();
            var sheet = await testingContext.Sheets.FirstAsync(s => s.AllowOver18 == false);
            var sheetForm = new EditSheetViewModel
                { Id = sheet.Id, AllowOver18 = true, CustomAgeLimit = 12, PostAgeLimit = PostAgeLimit.Custom };
            var cmd = new EditSheet.Command(sheet.Id, sheetForm);
            var handler = new EditSheet.CommandHandler(handlerContext);

            await handler.Handle(cmd, CancellationToken.None);
            var updated =
                await _fixture.CreateContext()
                    .Sheets.FirstAsync(s =>
                        s.Id == sheet.Id); // Do not use .FindAsync because it will return the 'sheet' version

            Assert.NotEqual(expected: 12, sheet.PostAgeLimitInDays);
            Assert.Equal(expected: 12, updated.PostAgeLimitInDays);
            Assert.True(updated.AllowOver18);
        }
    }
}
