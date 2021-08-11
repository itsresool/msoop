using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Msoop.Data;

namespace Msoop.Features.Subreddits
{
    public class DeleteSubreddit
    {
        public class Command : IRequest
        {
            // These properties need to have the same name as Subreddit/EditDelete.cshtml custom url
            public Guid SheetId { get; init; }
            public string SubName { get; init; }
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
                var subreddit = await _db.Subreddits.FindAsync(cmd.SheetId, cmd.SubName);
                if (subreddit is null)
                {
                    throw new InvalidOperationException("Impossible to remove it. This sheet has no such subreddit.");
                }

                _db.Remove(subreddit);
                await _db.SaveChangesAsync(cancellationToken);

                return default;
            }
        }
    }
}
