using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Msoop.Features.Subreddits;
using Msoop.ViewModels;

namespace Msoop.Pages.Sheets.Subreddits
{
    public class Create : PageModel
    {
        private readonly IMediator _mediator;

        public Create(IMediator mediator)
        {
            _mediator = mediator;
        }

        [BindProperty]
        public CreateSubredditViewModel Data { get; set; } = new();

        public async Task<ActionResult> OnPostAsync(Guid sheetId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var cmd = new CreateSubreddit.Command(sheetId, Data);
            var result = await _mediator.Send(cmd);

            switch (result)
            {
                case CreateSubreddit.Response.SubredditNotFound:
                    ModelState.AddModelError(nameof(Data), "Subreddit does not exist");
                    return Page();
                case CreateSubreddit.Response.SubredditAlreadyAdded:
                    ModelState.AddModelError(nameof(Data), "Subreddit already belongs to this sheet");
                    return Page();
                default:
                    return RedirectToPage("../EditDelete", new { id = sheetId });
            }
        }
    }
}
