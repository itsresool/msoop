using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Msoop.Web.Reddit.Exceptions;

namespace Msoop.Web.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        private readonly ILogger<ErrorModel> _logger;

        public ErrorModel(ILogger<ErrorModel> logger)
        {
            _logger = logger;
        }

        public string ErrorMessage { get; set; }

        public ActionResult OnGet()
        {
            return HandleError();
        }

        public ActionResult OnPost()
        {
            return HandleError();
        }

        private ActionResult HandleError()
        {
            var exceptionHandler = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var error = exceptionHandler?.Error;
            switch (error)
            {
                case null:
                    return RedirectToPage("/Index");
                case RateLimitedException rlError:
                    ErrorMessage = "Too many requests. Please wait a few minutes before you try again.";
                    _logger.LogError("You are rate limited, try again at: {AgainAtUtc}", rlError.AttemptAgainAtUtc);
                    break;
                case InvalidOperationException opError:
                    ErrorMessage = opError.Message;
                    break;
                case RedditServiceException:
                    ErrorMessage = "Problem with accessing reddit.";
                    break;
                default:
                    ErrorMessage = error.Message;
                    break;
            }

            return Page();
        }
    }
}
