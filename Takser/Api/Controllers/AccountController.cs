using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Takser.App.Persistence.Repositories;
using Takser.Domain.Models;

namespace Takser.Api.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository mUserRepository;

        public AccountController(IUserRepository userRepository)
        {
            mUserRepository = userRepository;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl = "/")
        {
            return View(new LoginModel { ReturnUrl = returnUrl }); ;
        }

        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<IActionResult> Login(LoginModel model)
        //{
        //    //IUser user = mUserRepository.GetByUsernameAndPassword(model.Username, model.Password);
        //    //if (user == null)
        //    //    return Unauthorized();

        //    //List<Claim> claims = new List<Claim>
        //    //{
        //    //    new Claim(ClaimTypes.NameIdentifier, user.Id),
        //    //    new Claim(ClaimTypes.Name, user.Name),
        //    //};

        //    //ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        //    //ClaimsPrincipal principal = new ClaimsPrincipal(identity);

        //    //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
        //    //    principal,
        //    //    new AuthenticationProperties { IsPersistent = model.RememberLogin });
        //}
    }
}