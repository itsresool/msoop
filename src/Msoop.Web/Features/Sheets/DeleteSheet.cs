using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Msoop.Infrastructure.Data;

namespace Msoop.Web.Features.Sheets
{
    public class DeleteSheet
    {
        public class Command : IRequest
        {
            public Guid Id { get; init; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly MsoopContext _db;

            public Handler(MsoopContext db)
            {
                _db = db;
            }

            public async Task<Unit> Handle(Command cmd, CancellationToken cancellationToken)
            {
                var sheet = await _db.Sheets.FindAsync(cmd.Id);
                if (sheet is null)
                {
                    throw new InvalidOperationException("There is no sheet to delete.");
                }

                _db.Sheets.Remove(sheet);
                await _db.SaveChangesAsync(cancellationToken);

                return default;
            }
        }
    }
}
