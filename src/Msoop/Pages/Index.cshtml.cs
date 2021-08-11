using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Msoop.Data;
using Msoop.Features.Sheets;
using Msoop.Models;
using Msoop.Reddit;

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
            
            return RedirectToPage("Sheets/EditDelete", new {id = newSheetId});
        }
    }
}
