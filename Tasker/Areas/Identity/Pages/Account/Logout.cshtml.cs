using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Tasker.Areas.Identity.Data;

namespace Tasker.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<TaskerUser> mSignInManager;
        private readonly ILogger<LogoutModel> mLogger;

        public LogoutModel(SignInManager<TaskerUser> signInManager, ILogger<LogoutModel> logger)
        {
            mSignInManager = signInManager;
            mLogger = logger;
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await mSignInManager.SignOutAsync().ConfigureAwait(false);
            mLogger.LogInformation("User logged out.");

            if (returnUrl != null)
                return LocalRedirect(returnUrl);
            else
                return RedirectToPage();
        }
    }
}