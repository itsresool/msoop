using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Msoop.Data;
using Msoop.ViewModels;

namespace Msoop.Pages.Sheets
{
    public class EditDelete : PageModel
    {
        private readonly MsoopContext _db;

        public EditDelete(MsoopContext db)
        {
            _db = db;
        }

        [BindProperty]
        public SheetFormViewModel Data { get; set; }

        public async Task<ActionResult> OnGetAsync(Guid id)
        {
            var sheet = await _db.Sheets.Include(s => s.Subreddits).FirstOrDefaultAsync(s => s.Id == id);
            if (sheet is null)
            {
                return RedirectToPage("/Index");
            }

            Data = new()
            {
                Id = sheet.Id,
                PostAgeLimit = sheet.LinksAgeLimitInDays switch
                {
                    1 => PostAgeLimit.LastDay,
                    7 => PostAgeLimit.LastWeek,
                    31 => PostAgeLimit.LastMonth,
                    365 => PostAgeLimit.LastYear,
                    _ => PostAgeLimit.Custom
                },
                CustomAgeLimit = sheet.LinksAgeLimitInDays,
                Subreddits = sheet.Subreddits.Select(sub => new CreateSubredditViewModel()
                    {
                        Name = sub.Name,
                        PostLimit = sub.PostLimit,
                        PostOrdering = sub.PostOrdering,
                    })
                    .ToList()
            };

            return Page();
        }

        public async Task<ActionResult> OnPostEditAsync(Guid id)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("invalid");
                return Page();
            }

            var sheet = await _db.Sheets.FindAsync(id);
            if (sheet is null)
            {
                return RedirectToPage("Index", new {id});
            }

            sheet.LinksAgeLimitInDays = Data.PostAgeLimit switch
            {
                PostAgeLimit.LastDay => 1,
                PostAgeLimit.LastWeek => 7,
                PostAgeLimit.LastMonth => 31,
                PostAgeLimit.LastYear => 365,
                PostAgeLimit.Custom => Data.CustomAgeLimit,
                _ => throw new ArgumentOutOfRangeException()
            };

            await _db.SaveChangesAsync();

            return RedirectToPage("Index", new {id});
        }

        public async Task<ActionResult> OnPostDeleteAsync(Guid id)
        {
            var sheet = await _db.Sheets.FindAsync(id);
            if (sheet is null)
            {
                // TODO show error page   
            }

            _db.Sheets.Remove(sheet);
            await _db.SaveChangesAsync();

            return RedirectToPage("/Index");
        }
    }
}
