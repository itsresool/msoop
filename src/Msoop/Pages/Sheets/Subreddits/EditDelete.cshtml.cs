using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Msoop.Data;
using Msoop.ViewModels;

namespace Msoop.Pages.Sheets.Subreddits
{
    public class EditDelete : PageModel
    {
        private readonly MsoopContext _db;

        public EditDelete(MsoopContext db)
        {
            _db = db;
        }

        [BindProperty]
        public EditDeleteSubredditViewModel Data { get; set; }

        public async Task<ActionResult> OnGetAsync(Guid sheetId, string subName)
        {
            var subreddit = await _db.Subreddits.FindAsync(sheetId, subName);
            if (subreddit is null)
            {
                return RedirectToPage("/Index");
            }

            Data = new()
            {
                Name = subreddit.Name,
                MaxPostCount = subreddit.MaxPostCount,
                PostOrdering = subreddit.PostOrdering
            };
            return Page();
        }

        public async Task<ActionResult> OnPostEditAsync(Guid sheetId, string subName)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var subreddit = await _db.Subreddits.FindAsync(sheetId, subName);
            subreddit.MaxPostCount = Data.MaxPostCount;
            subreddit.PostOrdering = Data.PostOrdering;
            await _db.SaveChangesAsync();

            return RedirectToPage("../EditDelete", new {Id = sheetId});
        }

        public async Task<ActionResult> OnPostDeleteAsync(Guid sheetId, string subName)
        {
            var subreddit = await _db.Subreddits.FindAsync(sheetId, subName);
            if (subreddit is null)
            {
                // TODO show error page   
            }

            _db.Remove(subreddit);
            await _db.SaveChangesAsync();

            return RedirectToPage("../EditDelete", new {Id = sheetId});
        }
    }
}
