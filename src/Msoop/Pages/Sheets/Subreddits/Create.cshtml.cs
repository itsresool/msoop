using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Msoop.Data;
using Msoop.Reddit;
using Msoop.ViewModels;

namespace Msoop.Pages.Sheets.Subreddits
{
    public class Create : PageModel
    {
        private readonly MsoopContext _db;
        private readonly RedditService _redditService;

        public Create(MsoopContext db, RedditService redditService)
        {
            _db = db;
            _redditService = redditService;
        }

        [BindProperty]
        public CreateSubredditViewModel Data { get; set; } = new();

        public async Task<ActionResult> OnPostAsync(Guid sheetId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!await _redditService.SubredditExists(Data.Name))
            {
                ModelState.AddModelError(nameof(Data), "Subreddit does not exist");
                return Page();
            }

            if (await _db.Subreddits.AnyAsync(sub => sub.Name == Data.Name && sub.SheetId == sheetId))
            {
                ModelState.AddModelError(nameof(Data), "Subreddit already belongs to this sheet");
                return Page();
            }

            var section = new Models.Subreddit
            {
                SheetId = sheetId,
                Name = Data.Name,
                MaxPostCount = Data.MaxPostCount,
                PostOrdering = Data.PostOrdering
            };
            _db.Subreddits.Add(section);
            await _db.SaveChangesAsync();

            return RedirectToPage("../EditDelete", new {id = sheetId});
        }
    }
}
