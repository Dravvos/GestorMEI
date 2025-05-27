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


var jwtSecret = builder.Configuration.GetSection("JwtSettings:Secret");
var issuer = builder.Configuration.GetSection("JwtSettings:Issuer");
var audience = builder.Configuration.GetSection("JwtSettings:Audience");

if (string.IsNullOrEmpty(jwtSecret.Value))
{
    throw new InvalidOperationException("JWT SECRET IS NOT SET");
}
if (string.IsNullOrEmpty(issuer.Value))
{
    throw new InvalidOperationException("JWT ISSUER IS NOT SET");
}
if (string.IsNullOrEmpty(audience.Value))
{
    throw new InvalidOperationException("JWT AUDIENCE IS NOT SET");
}


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = issuer.Value,
        ValidAudience = audience.Value,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret.Value))
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.TryGetValue("AuthToken", out var token))
            {
                context.Token = token;

            }
            return Task.CompletedTask;
        }
    };
});

if (builder.Environment.IsProduction())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", builder =>
        {
            builder.WithOrigins("https://www.meicaixa.com.br", "https://meicaixa.com.br")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials();
        });
    });

    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options => {
            options.Cookie.Name = "AuthToken";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // HTTPS only
            options.Cookie.SameSite = SameSiteMode.Lax; // or lax
            options.ExpireTimeSpan = TimeSpan.FromHours(3);
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
/*
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/shared/keys"))
    .SetApplicationName("SharedGestorMEI");
*/
builder.Services.AddScoped<IDBInitializer, DBInitializer>();
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "X-CSRF-TOKEN";
});

var app = builder.Build();

app.UseMiddleware<CustomMiddleware>();
app.UseRequestLocalization(localizationOptions);

if (app.Environment.IsDevelopment())
{
    app.UseCors(cors =>
    {
        cors.AllowAnyHeader();
        cors.AllowAnyMethod();
        cors.WithOrigins("http://localhost:5173", "https://localhost:44327");
        cors.AllowCredentials();
    });
}
else
{
    app.UseCors("AllowAll");
}


using var scope = app.Services.CreateScope();
var initializer = scope.ServiceProvider.GetRequiredService<IDBInitializer>();
initializer.Initialize();

app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
