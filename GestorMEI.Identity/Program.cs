using GestorMEI.Identity;
using GestorMEI.Identity.Initializer;
using GestorMEI.Identity.Models;
using GestorMEI.Identity.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

if (builder.Environment.IsProduction())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenLocalhost(5017);
    });
}

var supportedCultures = new[] { "pt" };

var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);


builder.Services.AddDbContext<PostgresContext>(options =>
{
    options.UseNpgsql(Environment.GetEnvironmentVariable("MEIConn"));
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<PostgresContext>()
    .AddDefaultTokenProviders()
    .AddErrorDescriber<LocalizedIdentityErrorDescriber>();

if (builder.Environment.IsProduction())
{
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options => {
            options.Cookie.Name = "AuthToken";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // HTTPS only
            options.Cookie.SameSite = SameSiteMode.Strict; // or lax
            options.ExpireTimeSpan = TimeSpan.FromHours(3);
            options.Cookie.Domain = "www.danieloliveira.net.br";
        });
}
else
{
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.Cookie.Name = "AuthToken";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.None; // or Strict
        options.ExpireTimeSpan = TimeSpan.FromHours(3);
    });
}

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;

});

builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddControllers();

builder.Services.AddAuthorization();

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/shared/keys"))
    .SetApplicationName("SharedGestorMEI");

builder.Services.AddScoped<IDBInitializer, DBInitializer>();
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "X-CSRF-TOKEN";
});

var app = builder.Build();
app.UseAntiforgery();


app.UseMiddleware<CustomMiddleware>();
app.UseRequestLocalization(localizationOptions);

if (app.Environment.IsDevelopment())
{
    app.UseCors(builder =>
        builder.WithOrigins("http://localhost:5173")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials());
}
else
{
    app.UseCors(builder =>
    builder.WithOrigins("https://www.danieloliveira.net.br")
           .AllowAnyHeader()
           .AllowAnyMethod()
           .AllowCredentials());
}


using var scope = app.Services.CreateScope();
var initializer = scope.ServiceProvider.GetRequiredService<IDBInitializer>();
initializer.Initialize();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
