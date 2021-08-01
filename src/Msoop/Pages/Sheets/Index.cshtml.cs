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

        public async Task<ActionResult> OnGet(Guid id)
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
            try
            {
                var fetchTasks = sheet.Subreddits.Select(async subreddit =>
                {
                    var listing = await _redditService.GetTopLinks(subreddit.Name, sheet.PostAgeLimitInDays);
                    var posts = listing.Data.Children.Select(l => l.Data)
                        .Where(p => (DateTimeOffset.UtcNow - p.CreatedUtc).Days <= sheet.PostAgeLimitInDays);

                    var orderedPosts = subreddit.PostOrdering switch
                    {
                        PostOrdering.Newest => posts.OrderByDescending(p => p.CreatedUtc),
                        PostOrdering.Oldest => posts.OrderBy(p => p.CreatedUtc),
                        PostOrdering.ScoreDesc => posts,
                        PostOrdering.CommentsDesc => posts.OrderByDescending(p => p.CommentsCount),
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    return new SubredditViewModel()
                    {
                        Name = subreddit.Name,
                        PostOrdering = subreddit.PostOrdering,
                        Posts = orderedPosts.Take(subreddit.MaxPostCount)
                    };
                });

                var fetchedSubreddits = await Task.WhenAll(fetchTasks);
                Data.AddRange(fetchedSubreddits);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
