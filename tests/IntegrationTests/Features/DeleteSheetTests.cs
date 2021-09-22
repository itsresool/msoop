using System;
using System.Threading;
using System.Threading.Tasks;
using Msoop.Web.Features.Sheets;
using Xunit;

namespace IntegrationTests.Features
{
    [Collection("MsoopTests")]
    public class DeleteSheetTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;

        public DeleteSheetTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Handler_ThrowsInvalidOperationException_WhenGivenIdIsNotInDb()
        {
            await using var handlerContext = _fixture.CreateContext();
            var cmd = new DeleteSheet.Command { Id = new Guid() };
            var handler = new DeleteSheet.Handler(handlerContext);

            await Assert.ThrowsAsync<InvalidOperationException>(()
                => handler.Handle(cmd, CancellationToken.None));
        }
    }
}
