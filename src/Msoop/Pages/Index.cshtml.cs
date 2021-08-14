using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Msoop.Features.Sheets;

namespace Msoop.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IMediator _mediator;

        public IndexModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<RedirectToPageResult> OnPostAsync()
        {
            var newSheetId = await _mediator.Send(new CreateSheet.Command());

            return RedirectToPage("Sheets/EditDelete", new { id = newSheetId });
        }
    }
}
