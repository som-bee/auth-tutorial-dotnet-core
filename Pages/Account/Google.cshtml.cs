using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthenticationTutorial.Pages.Account
{
    [AllowAnonymous]
    public class GoogleModel : PageModel
    {
        public /*async Task<IActionResult>*/ IActionResult OnGet(string ReturnUrl)
        {
            //await HttpContext.ChallengeAsync("Google");

            if (ReturnUrl == null)
            {
                ReturnUrl = "/";
            }

            if(!Url.IsLocalUrl(ReturnUrl))
            {
                throw new Exception("Invalid Return URL!");
            }

            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Page("/Callback"),
                Items =
                {
                    {"uru", ReturnUrl },
                    {"scheme","Google" },
                }   
            };


            return Challenge(props, "Google");
        }
    }
}
