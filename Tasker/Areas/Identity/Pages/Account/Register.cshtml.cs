using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Tasker.Areas.Identity.Data;

namespace Tasker.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<TaskerUser> mSignInManager;
        private readonly UserManager<TaskerUser> mUserManager;
        private readonly IEmailSender mEmailSender;
        private readonly ILogger<RegisterModel> mLogger;

        public RegisterModel(
            UserManager<TaskerUser> userManager,
            SignInManager<TaskerUser> signInManager,
            IEmailSender emailSender,
            ILogger<RegisterModel> logger)
        {
            mUserManager = userManager;
            mSignInManager = signInManager;
            mEmailSender = emailSender;
            mLogger = logger;
        }

        [BindProperty]
        public RegisterInputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins =
                (await mSignInManager.GetExternalAuthenticationSchemesAsync().ConfigureAwait(false)).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins =
                (await mSignInManager.GetExternalAuthenticationSchemesAsync().ConfigureAwait(false)).ToList();
            if (ModelState.IsValid)
            {
                var user = new TaskerUser { UserName = Input.Email, Email = Input.Email };
                var result = await mUserManager.CreateAsync(user, Input.Password).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    mLogger.LogInformation("User created a new account with password.");

                    var code =
                        await mUserManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code, returnUrl },
                        protocol: Request.Scheme);

                    await mEmailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.")
                        .ConfigureAwait(false);

                    if (mUserManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl });
                    }
                    else
                    {
                        await mSignInManager.SignInAsync(user, isPersistent: false).ConfigureAwait(false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
