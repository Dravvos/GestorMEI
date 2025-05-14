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
using GestorMEI.API;

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
builder.Services.AddControllers();


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

app.UseMiddleware<CustomMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    app.UseCors(builder =>
        builder.WithOrigins("http://localhost:5173", "https://localhost:44327")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials());
}
else
{
    app.UseCors(cors =>
    {
        cors.AllowAnyHeader();
        cors.AllowAnyMethod();
        cors.AllowAnyOrigin();
        cors.AllowCredentials();
    });
}


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
