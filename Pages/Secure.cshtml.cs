using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace AuthenticationTutorial.Pages
{
    [Authorize]
    public class SecureModel : PageModel
    {
        public SessionData Session { get; set; }
        public async void OnGet()
        {
            //retrive current session
            var authResult = await HttpContext.AuthenticateAsync();

            var userClaims = authResult.Principal.Claims;
            var metadata = authResult.Properties.Items;

            Session = new SessionData(userClaims, metadata);
        }

        public record SessionData(
            IEnumerable<Claim> Claims,
            IDictionary<string,string> MetaData
            );

    }
}
