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



//auth scheme configurationz
builder.Services.AddAuthentication("cookie")
    .AddCookie("cookie", o =>
    {
        o.Cookie.Name = "demo";
        o.ExpireTimeSpan = TimeSpan.FromDays(1);

        //redirection path when challenged
        o.LoginPath = "/account/login"; 

    })
    .AddCookie("temp")
    .AddGoogle("Google", o =>
    {
        o.ClientId= googleClientId;
        o.ClientSecret = googleClientSecret;
        //path for listening for callback
        //o.CallbackPath = "/signin-google"; // default

        //after google handler is done cookie handler / default handler will be called to sign in the user 
        //and start the session
        // o.SignInScheme = "cookie"; //default
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
