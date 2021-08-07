using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Msoop.Features.Sheets;
using Msoop.ViewModels;

namespace Msoop.Pages.Sheets
{
    public class EditDelete : PageModel
    {
        private readonly IMediator _mediator;

        public EditDelete(IMediator mediator)
        {
            _mediator = mediator;
        }

        [BindProperty]
        public EditSheetViewModel Form { get; set; }

        public IList<SubredditSummaryViewModel> OwnedSubreddits { get; set; }

        public async Task<ActionResult> OnGetAsync(GetSheet.Query query)
        {
            var result = await _mediator.Send(query);
            if (result is null) return RedirectToPage("/Index");

            Form = result.Sheet;
            OwnedSubreddits = result.OwnedSubreddits;

            return Page();
        }

        public async Task<ActionResult> OnPostEditAsync(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var cmd = new EditSheet.Command(id, Form);
            await _mediator.Send(cmd);

            return RedirectToPage("Index", new {id});
        }

        public async Task<ActionResult> OnPostDeleteAsync(DeleteSheet.Command cmd)
        {
            await _mediator.Send(cmd);

            return RedirectToPage("/Index");
        }
    }
}
