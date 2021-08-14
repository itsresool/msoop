using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Msoop.Features.Subreddits;
using Msoop.ViewModels;

namespace Msoop.Pages.Sheets.Subreddits
{
    public class EditDelete : PageModel
    {
        private readonly IMediator _mediator;

        public EditDelete(IMediator mediator)
        {
            _mediator = mediator;
        }

        [BindProperty]
        public EditSubredditViewModel Data { get; set; }

        public async Task<ActionResult> OnGetAsync(EditSubreddit.Query query)
        {
            Data = await _mediator.Send(query);
            if (Data is null)
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }

        public async Task<ActionResult> OnPostEditAsync(EditSubreddit.Query query)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var cmd = new EditSubreddit.Command(query, Data);
            await _mediator.Send(cmd);

            return RedirectToPage("../EditDelete", new { Id = cmd.SheetId });
        }

        public async Task<ActionResult> OnPostDeleteAsync(DeleteSubreddit.Command cmd)
        {
            await _mediator.Send(cmd);

            return RedirectToPage("../EditDelete", new { Id = cmd.SheetId });
        }
    }
}
