using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Msoop.Data;
using Msoop.Models;

namespace Msoop.Features.Sheets
{
    public class CreateSheet
    {
        public class Command : IRequest<Guid>
        {
        }

        public class Handler : IRequestHandler<Command, Guid>
        {
            private readonly MsoopContext _db;

            public Handler(MsoopContext db)
            {
                _db = db;
            }

            public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
            {
                var sheet = new Sheet
                {
                    AllowOver18 = true,
                    AllowSpoilers = true,
                    AllowStickied = false
                };

                _db.Sheets.Add(sheet);
                await _db.SaveChangesAsync(cancellationToken);

                return sheet.Id;
            }
        }
    }
}
