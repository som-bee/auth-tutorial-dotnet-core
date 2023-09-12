using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace AuthenticationTutorial.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public String Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty(SupportsGet =true)]
        public String ReturnUrl { get; set; }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (!string.IsNullOrWhiteSpace(Username) && Username == Password)
            {
                var claims = new List<Claim>
                {
                    new ("sub","123"),
                    new("username","sombee"),
                    new("role","admin")
                };

                var claimsId = new ClaimsIdentity(claims, "pwd", "username","role");

                var claimsPrin = new ClaimsPrincipal(claimsId);

                await HttpContext.SignInAsync(claimsPrin);
                if(ReturnUrl == null)
                {
                    ReturnUrl = "/";
                }
                return LocalRedirect(ReturnUrl);

            }
            return Page();
        }
    }
}
