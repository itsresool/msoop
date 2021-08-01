using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Msoop.Data;
using Msoop.ViewModels;

namespace Msoop.Pages.Sheets.Subreddits
{
    public class Create : PageModel
    {
        private readonly MsoopContext _db;

        public Create(MsoopContext db)
        {
            _db = db;
        }

        [BindProperty]
        public CreateSubredditViewModel Data { get; set; } = new();

        public async Task<ActionResult> OnPostAsync(Guid sheetId)
        {
            if (!ModelState.IsValid)
            {
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
