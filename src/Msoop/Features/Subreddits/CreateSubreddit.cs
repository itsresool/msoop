using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Msoop.Data;
using Msoop.Models;
using Msoop.Reddit;
using Msoop.ViewModels;

namespace Msoop.Features.Subreddits
{
    public class CreateSubreddit
    {
        public enum Response
        {
            Ok,
            SubredditNotFound,
            SubredditAlreadyAdded
        }

        public class Command : IRequest<Response>
        {
            public Guid SheetId { get; }
            public CreateSubredditViewModel Form { get; }

            public Command(Guid sheetId, CreateSubredditViewModel form)
            {
                SheetId = sheetId;
                Form = form;
            }
        }

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly MsoopContext _db;
            private readonly RedditService _redditService;

            public Handler(MsoopContext db, RedditService redditService)
            {
                _db = db;
                _redditService = redditService;
            }

            public async Task<Response> Handle(Command cmd, CancellationToken cancellationToken)
            {
                if (!await _redditService.SubredditExists(cmd.Form.Name))
                {
                    return Response.SubredditNotFound;
                }

                if (await _db.Subreddits.AnyAsync(sub => sub.Name == cmd.Form.Name && sub.SheetId == cmd.SheetId,
                    cancellationToken))
                {
                    return Response.SubredditAlreadyAdded;
                }

                var subreddit = new Subreddit
                {
                    SheetId = cmd.SheetId,
                    Name = cmd.Form.Name,
                    MaxPostCount = cmd.Form.MaxPostCount,
                    PostOrdering = cmd.Form.PostOrdering,
                };

                _db.Subreddits.Add(subreddit);
                await _db.SaveChangesAsync(cancellationToken);

                return Response.Ok;
            }
        }
    }
}
