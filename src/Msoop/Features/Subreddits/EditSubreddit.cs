using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Msoop.Data;
using Msoop.ViewModels;

namespace Msoop.Features.Subreddits
{
    public class EditSubreddit
    {
        public class Query : IRequest<EditSubredditViewModel>
        {
            // These properties need to have the same name as Subreddit/EditDelete.cshtml custom url
            public Guid SheetId { get; init; }
            public string SubName { get; init; }
        }

        public class QueryHandler : IRequestHandler<Query, EditSubredditViewModel>
        {
            private readonly MsoopContext _db;
            private readonly IMapper _mapper;

            public QueryHandler(MsoopContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<EditSubredditViewModel> Handle(Query msg, CancellationToken cancellationToken)
            {
                var subreddit = await _db.Subreddits.FindAsync(msg.SheetId, msg.SubName);
                return subreddit is not null ? _mapper.Map<EditSubredditViewModel>(subreddit) : null;
            }
        }

        public class Command : IRequest
        {
            public Guid SheetId { get; }
            public string SubName { get; set; }
            public EditSubredditViewModel Form { get; }

            public Command(Query query, EditSubredditViewModel form)
            {
                SheetId = query.SheetId;
                SubName = query.SubName;
                Form = form;
            }
        }

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly MsoopContext _db;

            public CommandHandler(MsoopContext db)
            {
                _db = db;
            }

            public async Task<Unit> Handle(Command cmd, CancellationToken cancellationToken)
            {
                var subreddit = await _db.Subreddits.FindAsync(cmd.SheetId, cmd.SubName);
                if (subreddit is null)
                {
                    throw new InvalidOperationException(
                        "Impossible to save. This sheet has no such subreddit.");
                }

                subreddit.MaxPostCount = cmd.Form.MaxPostCount;
                subreddit.PostOrdering = cmd.Form.PostOrdering;
                await _db.SaveChangesAsync(cancellationToken);

                return default;
            }
        }
    }
}
