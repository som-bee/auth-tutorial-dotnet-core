using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthenticationTutorial.Pages
{
    [Authorize]
    public class SecureModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
