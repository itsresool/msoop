using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Msoop.Features.Sheets;
using Msoop.ViewModels;

namespace Msoop.Pages.Sheets
{
    public class Index : PageModel
    {
        private readonly IMediator _mediator;

        public Index(IMediator mediator)
        {
            _mediator = mediator;
        }

        public DetailedSheetViewModel Data { get; set; }

        public async Task<ActionResult> OnGetAsync(GetDetailedSheet.Query query)
        {
            Data = await _mediator.Send(query);
            if (Data is null)
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }
    }
}
