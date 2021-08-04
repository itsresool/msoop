using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Msoop.Data;
using Msoop.Models;
using Msoop.Reddit;

namespace Msoop.Pages
{
    public class IndexModel : PageModel
    {
        private readonly MsoopContext _db;

        public IndexModel(MsoopContext db)
        {
            _db = db;
        }

        public async Task<RedirectToPageResult> OnPostAsync()
        {
            var sheet = new Sheet()
            {
                AllowOver18 = true,
                AllowSpoilers = true,
                AllowStickied = false
            };

            _db.Sheets.Add(sheet);
            await _db.SaveChangesAsync();

            return RedirectToPage("Sheets/EditDelete", new {id = sheet.Id});
        }
    }
}
