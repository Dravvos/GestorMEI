using GestorMEI.API;
using GestorMEI.BLL.Repositories;
using GestorMEI.BLL.Repositories.Interfaces;
using GestorMEI.BLL.Services;
using GestorMEI.BLL.Services.Interfaces;
using GestorMEI.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MeiContext>(options =>
{
    var connection = Environment.GetEnvironmentVariable("MEIConn");
    options.UseNpgsql(connection);
});

var jwtSecret = builder.Configuration.GetSection("JwtSettings:Secret");

if (string.IsNullOrEmpty(jwtSecret.Value))
{
    throw new InvalidOperationException("JWT SECRET IS NOT SET");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret.Value)),
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

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Digite 'Bearer' [espaço] e seu token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
    {
        new OpenApiSecurityScheme
        {
            Reference=new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            },
            Scheme = "oauth2",
            Name = "Bearer",
            In = ParameterLocation.Header
       },
        new List<string>()
    }
    });
});

if (builder.Environment.IsProduction())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", builder =>
        {
            builder.WithOrigins("https://www.meicaixa.com.br", "https://meicaixa.com.br", "https://www.danieloliveira.net.br", "https://danieloliveira.net.br")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials();
        });
    });
}

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

app.UseMiddleware<CustomMiddleware>();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseSwagger();
app.UseSwaggerUI();

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
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
