using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Msoop.Data;
using Msoop.ViewModels;
using Msoop.Models;
using Msoop.Reddit;

namespace Msoop.Pages.Sheets
{
    public class Index : PageModel
    {
        private readonly MsoopContext _db;
        private readonly RedditService _redditService;

        public Index(MsoopContext db, RedditService redditService)
        {
            _db = db;
            _redditService = redditService;
        }

        public List<SubredditViewModel> Data { get; set; } = new();

        public async Task<ActionResult> OnGetAsync(Guid id)
        {
            var sheet = await _db.Sheets.Include(s => s.Subreddits).FirstOrDefaultAsync(s => s.Id == id);
            if (sheet is null)
            {
                return RedirectToPage("/Index");
            }

            await FetchSheetData(sheet);

            return Page();
        }

        private async Task FetchSheetData(Sheet sheet)
        {
            var fetchTasks = sheet.Subreddits.Select(async subreddit =>
            {
                var listingCmd = new RedditService.ListingCommand
                {
                    SubredditName = subreddit.Name,
                    MaxPostCount = subreddit.MaxPostCount,
                    PostAgeLimitInDays = sheet.PostAgeLimitInDays
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
                    _ => throw new ArgumentOutOfRangeException(nameof(subreddit.PostOrdering))
                };

                return new SubredditViewModel()
                {
                    Name = subreddit.Name,
                    PostOrdering = subreddit.PostOrdering,
                    Posts = orderedPosts
                };
            });

            var fetchedSubreddits = await Task.WhenAll(fetchTasks);
            Data.AddRange(fetchedSubreddits);
        }
    }
}
