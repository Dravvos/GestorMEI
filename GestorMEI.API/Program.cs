using GestorMEI.BLL.Repositories;
using GestorMEI.BLL.Repositories.Interfaces;
using GestorMEI.BLL.Services.Interfaces;
using GestorMEI.BLL.Services;
using GestorMEI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MeiContext>(options =>
{
    var connection = builder.Configuration["MEIConn"];
    options.UseNpgsql(connection);
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!)),
            ValidateLifetime = true
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["AuthToken"];
                if (!string.IsNullOrEmpty(token))
                    context.Token = token;

                return Task.CompletedTask;
            }
        };
    });
/*
if (builder.Environment.IsProduction())
{
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.Cookie.Name = "AuthToken";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // HTTPS only
            options.Cookie.SameSite = SameSiteMode.Strict; // or lax
            options.ExpireTimeSpan = TimeSpan.FromHours(3); // Short-lived token
            //options.LoginPath = "https://www.danieloliveira.net.br/GestorMEI.Identity/api/Auth/Login";
        });
    builder.Services.AddAntiforgery(options =>
    {
        options.Cookie.Domain = ".danieloliveira.net.br";
        options.Cookie.Name = "X-CSRF-TOKEN";
        options.HeaderName = "X-XSRF-TOKEN";
    });
}
else
{
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "AuthToken";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.None; // or Strict
        options.ExpireTimeSpan = TimeSpan.FromHours(3); // Short-lived token
        //options.LoginPath = "/api/Auth/Login";
    });

    builder.Services.AddAntiforgery(options =>
    {
        options.HeaderName = "X-XSRF-TOKEN";
        options.Cookie.Name = "X-CSRF-TOKEN";
    });
}
*/
builder.Services.AddControllers();
/*
builder.Services.AddAuthentication("Bearer").AddJwtBearer(options =>
{
    options.Authority = "https://localhost:44327/api/";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false,
    };
});
*/


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAssinaturaRepository, AssinaturaRepository>();
builder.Services.AddScoped<IEmpresaRepository, EmpresaRepository>();
builder.Services.AddScoped<IVendaRepository, VendaRepository>();
builder.Services.AddScoped<ITabelaGeralRepository, TabelaGeralRepository>();
builder.Services.AddScoped<ITabelaGeralItemRepository, TabelaGeralItemRepository>();

builder.Services.AddScoped<IAssinaturaService, AssinaturaService>();
builder.Services.AddScoped<IEmpresaService, EmpresaService>();
builder.Services.AddScoped<IVendaService, VendaService>();
builder.Services.AddScoped<ITabelaGeralService, TabelaGeralService>();
builder.Services.AddScoped<ITabelaGeralItemService, TabelaGeralItemService>();
builder.Services.AddScoped<IRelatorioService, RelatorioService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.UseAntiforgery();

app.UseCors(builder =>
    builder.WithOrigins("http://localhost:5173","https://localhost:44327")
           .AllowAnyHeader()
           .AllowAnyMethod()
           .AllowCredentials());

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
