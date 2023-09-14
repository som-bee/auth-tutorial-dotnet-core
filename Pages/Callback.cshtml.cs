using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace AuthenticationTutorial.Pages
{
    [AllowAnonymous]
    public class CallbackModel : PageModel
    {
        public async Task<IActionResult> OnGet()
        {
            //read the outcome of external auth
            var result = await HttpContext.AuthenticateAsync("temp"); // returns the temps session and auth result

            if (!result.Succeeded)
            {
                throw new Exception("External Auth Failed!");
            }


            //getting the user from temp Cookie
            var extUser = result.Principal;

            //id of the user of external provider
            var subId = extUser.FindFirst(ClaimTypes.NameIdentifier).Value;

            //provider name passed as props
            var issuer = result.Properties.Items["scheme"];



            //run our logic
            //1st time user vs returning user


            //sign-in the user into application
            var claims = new List<Claim>
                {
                    new ("sub","123"),
                    new("name",extUser.FindFirst(ClaimTypes.Name).Value),
                    new("email",extUser.FindFirst(ClaimTypes.Email).Value),
                    new("role","User")
                };

            var claimsId = new ClaimsIdentity(claims, issuer, "name", "role");

            var claimsPrin = new ClaimsPrincipal(claimsId);

            await HttpContext.SignInAsync(claimsPrin);
            await HttpContext.SignOutAsync("temp");

            //ultimate return url
            var uru = result.Properties.Items["uru"];

            if (uru == null)
            {
                uru = "/";
            }
            return LocalRedirect(uru);

        }
    }
}
