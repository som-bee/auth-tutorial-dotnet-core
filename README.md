# AuthenticationTutorial







Authentication - ASP.Net core
SOURCE : [Overview :: Duende IdentityServer Documentation](https://docs.duendesoftware.com/identityserver/v6/overview/)
Official Repo : github.com/blowdart/AspNetAuthorizationWorkshop
My Implementation : som-bee/auth-tutorial-dotnet-core








USING SESSION COOKIES
Authentication HttpContext Convenience extension methods: 
YT : Basics Part 1: Introduction to ASP.NET Core Authentication & Authorization
src : AuthenticationHttpContextExtens…

Interaction with the authentication system :

















/Program.cs

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();



//auth scheme configuration
builder.Services.AddAuthentication("cookie")
    .AddCookie("cookie", o =>
    {
        o.Cookie.Name = "demo";
        o.ExpireTimeSpan = TimeSpan.FromDays(1);

        //redirection path when challenged
        o.LoginPath = "/account/login"; 

    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//authentication middleware
app.UseAuthentication();

// authorization middleware
app.UseAuthorization();

//Authorization required for all the pages
app.MapRazorPages().RequireAuthorization();



app.Run();


/Pages/Account/Login.cshtml

@page
@model AuthenticationTutorial.Pages.Account.LoginModel
@{
    ViewData["Title"] = "Login";
}
<div>


<h2>Login</h2>

<form method="post">
    <div class="form-group">
        <label for="Username">Username:</label>
        <input type="text" id="Username" name="Username" class="form-control" />
    </div>
    <div class="form-group">
        <label for="Password">Password:</label>
        <input type="password" id="Password" name="Password" class="form-control" />
    </div>
    <br />
    <button type="submit" class="btn btn-primary">Login</button>
</form>


</div>



/Pages/Account/Login.cshtml.cs

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
            }
            return Page();
        }
    }
}


EXTERNAL

Google OAuth 2.0 

YT: Basics Part 2a: Adding external Authentication to your ASP.NET Core Application
Basics Part 2b: The "external authentication callback" Pattern


ASP.NET OAuth Providers:   AspNet.Security.OAuth.Providers: OAuth 2.0 social authentication providers for ASP.NET Core




External Authentication:


User Authentication Flow:

/Pages/Account/Login.cshtml

@page
@model AuthenticationTutorial.Pages.Account.LoginModel
@{
    ViewData["Title"] = "Login";
}
<div>


<h2>Login</h2>

<form method="post">
    <div class="form-group">
        <label for="Username">Username:</label>
        <input type="text" id="Username" name="Username" class="form-control" />
    </div>
    <div class="form-group">
        <label for="Password">Password:</label>
        <input type="password" id="Password" name="Password" class="form-control" />
    </div>
    <br />
    <button type="submit" class="btn btn-primary">Login</button>
</form>
<a asp-page="Google" asp-route-returnurl="@Model.ReturnUrl">
    sign in with google
</a>

</div>



/Pages/Account/Google.cshtml.cs

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
//old
           /* var props = new AuthenticationProperties
            {
                RedirectUri = ReturnUrl,
            }; */
		
//new
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
/AuthenticationTutorial.csproj

<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net7.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>


	<ItemGroup>
		<PackageReference Include="Microsoft.AspNetCore.Authentication.Google" Version="7.0.0"/>
	</ItemGroup>

</Project>

Storing OAuth ClienId & Secret
User Secrets (Development Only): In your development environment, you can use the User Secrets feature provided by ASP.NET Core. This stores secrets outside of your project directory in a safe location.

To use User Secrets, you can right-click on your project in Visual Studio and select "Manage User Secrets." In the secrets.json file, you can store your secrets like client IDs and secrets:

/secret.json

{
  "GoogleClientId": "your-client-id",
  "GoogleClientSecret": "your-client-secret"
}


/Program.cs

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

//builder.Services.AddRazorPages(options =>
//{
//    options.Conventions.AddPageRoute("/Pages/Account/Login", "/account/login"); // Map /account/login to your login page.
//});


// Build the configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddUserSecrets<Program>() // Replace with your actual program type
    .Build();

// Access User Secrets
var googleClientId = configuration["GoogleClientId"];
var googleClientSecret = configuration["GoogleClientSecret"];



//auth scheme configuration
builder.Services.AddAuthentication("cookie")
    .AddCookie("cookie", o =>
    {
        o.Cookie.Name = "demo";
        o.ExpireTimeSpan = TimeSpan.FromDays(1);

        //redirection path when challenged
        o.LoginPath = "/account/login"; 

    })
    .AddCookie("temp")//temp session for storing external auth result
    .AddGoogle("Google", o =>
    {
        o.ClientId= googleClientId;
        o.ClientSecret = googleClientSecret;
        //path for listening for callback
        //o.CallbackPath = "/signin-google"; // default
        //after google handler is done cookie handler / default handler will be called to sign in the user and start the session
        //o.SignInScheme = "cookie"; //default
	  o.SignInScheme = "temp";
    })
    ;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//authentication middleware
app.UseAuthentication();

// authorization middleware
app.UseAuthorization();

app.MapRazorPages().RequireAuthorization();

app.Run();





/Pages/Callback.cshtml.cs

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





OpenID Connect 

YT: Basics Part 3: Using OpenID Connect for Authentication and API Access


Demo Clients: Duende IdentityServer (using client login in our implementation)



No need : loging page, callback, temp cookie, Oauth, 



/AuthenticationTutorial.csproj

<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net7.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>


	<ItemGroup>
		<PackageReference Include="Microsoft.AspNetCore.Authentication.OpenIdConnect" Version="7.0.0"/>
	</ItemGroup>

</Project>







/Secure.cshtml.cs

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




/Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();


// Build the configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddUserSecrets<Program>() // Replace with your actual program type
    .Build();

// Access User Secrets
var googleClientId = configuration["GoogleClientId"];
var googleClientSecret = configuration["GoogleClientSecret"];



//auth scheme configurationz
//builder.Services.AddAuthentication("cookie")
//for OpenId protocol
builder.Services.AddAuthentication(o =>
{
    o.DefaultScheme = "cookie";
    o.DefaultChallengeScheme = "oidc";
})
    .AddCookie("cookie", o =>
    {
        o.Cookie.Name = "demo";
        o.ExpireTimeSpan = TimeSpan.FromDays(1);

        //redirection path when challenged
        o.LoginPath = "/account/login"; 

    })
    //.AddCookie("temp")
    //.AddGoogle("Google", o =>
    //{
    //    o.ClientId= googleClientId;
    //    o.ClientSecret = googleClientSecret;
    //    //path for listening for callback
    //    //o.CallbackPath = "/signin-google"; // default

    //    //after google handler is done cookie handler / default handler will be called to sign in the user 
    //    //and start the session
    //    // o.SignInScheme = "cookie"; //default
    //    o.SignInScheme = "temp";
    //})
     .AddOpenIdConnect("oidc", o =>
    {
        o.Authority = "https://demo.duendesoftware.com";
        o.ClientId = "login";

        //scopes
        o.Scope.Clear();
        o.Scope.Add("openid");
        o.Scope.Add("profile");


        //for storing the id token that comes from openid auth -				// increases the size of cookies
//also makes the logout functionality seamless
        o.SaveTokens = true;


        //claimAction filters out unnecessary claims by default but can be cleared by
        //o.ClaimActions.Clear();


        //for getting raw  unmapped claims
        o.MapInboundClaims = false;
    })

    ;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//authentication middleware
app.UseAuthentication();

// authorization middleware
app.UseAuthorization();

app.MapRazorPages().RequireAuthorization();



app.Run();


/Account/Logout.cshtml.cs

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthenticationTutorial.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGet()
        {
            //await HttpContext.SignOutAsync();

            //return Page();

            return SignOut("cookie","oidc");
        }
    }
}






