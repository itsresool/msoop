using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Msoop.Data;
using Msoop.Models;
using Msoop.Reddit;
using Msoop.ViewModels;

namespace Msoop.Features.Sheets
{
    public class GetDetailedSheet
    {
        public class Query : IRequest<DetailedSheetViewModel>
        {
            public Guid Id { get; init; }
        }

        public class Handler : IRequestHandler<Query, DetailedSheetViewModel>
        {
            private readonly MsoopContext _db;
            private readonly RedditService _redditService;

            public Handler(MsoopContext db, RedditService redditService)
            {
                _db = db;
                _redditService = redditService;
            }

            public async Task<DetailedSheetViewModel> Handle(Query msg, CancellationToken cancellationToken)
            {
                var sheet = await _db.Sheets.Include(s => s.Subreddits)
                    .FirstOrDefaultAsync(s => s.Id == msg.Id, cancellationToken);
                if (sheet is null)
                {
                    return null;
                }

                var fetchTasks = sheet.Subreddits.Select(async subreddit =>
                {
                    var listingCmd = new RedditService.ListingCommand
                    {
                        SubredditName = subreddit.Name,
                        MaxPostCount = subreddit.MaxPostCount,
                        PostAgeLimitInDays = sheet.PostAgeLimitInDays,
                    };
                    var listing = await _redditService.GetTopListing(listingCmd);
                    var posts = listing.Data.Children.Select(l => l.Data)
                        .Where(p => (DateTimeOffset.UtcNow - p.CreatedUtc).Days <= sheet.PostAgeLimitInDays)
                        .Where(p => sheet.AllowOver18 || p.IsOver18 is false)
                        .Where(p => sheet.AllowSpoilers || p.IsSpoiler is false)
                        .Where(p => sheet.AllowStickied || p.IsStickied is false)
                        .Take(subreddit.MaxPostCount);

                    var orderedPosts = subreddit.PostOrdering switch
                    {
                        PostOrdering.Newest => posts.OrderByDescending(p => p.CreatedUtc),
                        PostOrdering.Oldest => posts.OrderBy(p => p.CreatedUtc),
                        PostOrdering.ScoreDesc => posts,
                        PostOrdering.CommentsDesc => posts.OrderByDescending(p => p.CommentsCount),
                        _ => throw new ArgumentOutOfRangeException(nameof(subreddit.PostOrdering)),
                    };

                    return new DetailedSheetViewModel.Subreddit
                    {
                        Name = subreddit.Name,
                        PostOrdering = subreddit.PostOrdering,
                        Posts = orderedPosts,
                    };
                });

                return new DetailedSheetViewModel
                {
                    Subreddits = await Task.WhenAll(fetchTasks),
                };
            }
        }
    }
}
