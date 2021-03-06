using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Msoop.Web.Features.Sheets;
using Msoop.Web.ViewModels;

namespace Msoop.Web.Pages.Sheets
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
