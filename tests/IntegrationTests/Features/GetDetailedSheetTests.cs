using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Msoop.Web.Features.Sheets;
using Msoop.Web.Reddit;
using Xunit;

namespace IntegrationTests.Features
{
    [Collection("MsoopTests")]
    public class GetDetailedSheetTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;

        public GetDetailedSheetTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Handler_ReturnsNull_WhenGivenSheetDoesNotExists()
        {
            await using var handlerContext = _fixture.CreateContext();
            var cmd = new GetDetailedSheet.Query { Id = new Guid() };
            var mockRedditService = new Mock<IRedditService>();
            var handler = new GetDetailedSheet.Handler(handlerContext, mockRedditService.Object);

            var actual = await handler.Handle(cmd, CancellationToken.None);

            Assert.Null(actual);
        }
    }
}
