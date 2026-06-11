using GestorMEI.DTO.Auth;
using GestorMEI.Identity.Configuration;
using GestorMEI.Identity.Models;
using GestorMEI.Identity.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;

namespace GestorMEI.Identity.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _service;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, ITokenService service)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _service = service;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAccount([FromBody] SignUpDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user != null)
            {
                return BadRequest("Email already in use");
            }
            var result = await _userManager.CreateAsync(new ApplicationUser { Nome = dto.Nome, Sobrenome = dto.Sobrenome, Email = dto.Email, AceitouOsTermosDeUsoPrivacidade = true, UserName = dto.Email }, dto.Password);
            if (result.Succeeded)
            {
                user = await _userManager.FindByEmailAsync(dto.Email);
                await _userManager.AddToRoleAsync(user, IdentityConfiguration.Client);
                await _userManager.AddClaimsAsync(user, new Claim[]
              {
                    new Claim(ClaimTypes.Email,dto.Email),
                    new Claim(ClaimTypes.GivenName,dto.Nome),
                    new Claim(ClaimTypes.Surname,dto.Sobrenome),
                    new Claim(ClaimTypes.Role, IdentityConfiguration.Client)
              });

                await SendConfirmationEmail(dto.Email);

                return Created();
            }
            else
                return BadRequest(result.Errors);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(dto.Username);
                if (user == null)
                {
                    user = await _userManager.FindByEmailAsync(dto.Username);
                    if (user == null)
                        return BadRequest("Invalid username/password");
                }

                var result = await _signInManager.PasswordSignInAsync(user.UserName, dto.Password, false, false);
                if (result.Succeeded)
                {
                    var token = await _service.GenerateTokenAsync(user, _userManager);
                    var claims = new List<Claim>
                {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName,user.Nome),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.Sobrenome),
                new Claim(JwtRegisteredClaimNames.Email,user.Email)
                };
                    var roles = await _userManager.GetRolesAsync(user);
                    foreach (var role in roles)
                    {
                        claims.Add(new Claim("role", role));
                        if (_roleManager.SupportsRoleClaims)
                        {
                            var identityRole = await _roleManager.FindByNameAsync(role);
                            if (identityRole != null)
                                claims.AddRange(await _roleManager.GetClaimsAsync(identityRole));
                        }
                    }
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));

                    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                    var isDevelopment = environment == Environments.Development;
                    var cookieOptions = new CookieOptions();
                    if (isDevelopment == false)
                    {
                        cookieOptions.HttpOnly = true;
                        cookieOptions.Secure = true;
                        cookieOptions.SameSite = SameSiteMode.Strict;
                        cookieOptions.Expires = DateTime.Now.AddHours(3);
                        cookieOptions.IsEssential = true; // Make the session cookie essential
                    }
                    else
                    {
                        cookieOptions.HttpOnly = true;
                        cookieOptions.Secure = true;
                        cookieOptions.SameSite = SameSiteMode.None;
                        cookieOptions.Expires = DateTime.Now.AddHours(3);
                        cookieOptions.IsEssential = true; // Make the session cookie essential
                    }

                    Response.Cookies.Append("AuthToken", token, cookieOptions);
                    return Ok("Logged in succesfully");
                }
                else
                    return BadRequest("Invalid credentials");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }

        [HttpPut]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var token = HttpContext.Request.Cookies["AuthToken"];
            if (token != null)
            {
                var cookieOptions = new CookieOptions();
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                var isDevelopment = environment == Environments.Development;
                if (isDevelopment == false)
                {
                    cookieOptions.HttpOnly = true;
                    cookieOptions.Secure = true;
                    cookieOptions.SameSite = SameSiteMode.Strict;
                    cookieOptions.Expires = DateTime.Now.AddDays(-1);
                    cookieOptions.IsEssential = true; // Make the session cookie essential
                }
                else
                {
                    cookieOptions.HttpOnly = true;
                    cookieOptions.Secure = true;
                    cookieOptions.SameSite = SameSiteMode.None;
                    cookieOptions.Expires = DateTime.Now.AddDays(-1);
                    cookieOptions.IsEssential = true; // Make the session cookie essential
                }
                HttpContext.Response.Cookies.Append("AuthToken", "", cookieOptions);
                return Ok("Logged out successfully");
            }
            return StatusCode(418, "User is not logged in");

        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAccount(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
                return NoContent();
            else
                return BadRequest(result.Errors);
        }

        [HttpGet]

        public IActionResult user()
        {
            try
            {
                HttpContext.Request.Cookies.TryGetValue("AuthToken", out var cookie);

                if (string.IsNullOrEmpty(cookie))
                    return Unauthorized();

                var decodedToken = new JwtSecurityTokenHandler().ReadJwtToken(cookie);
                var claims = decodedToken.Claims;

                string userId = claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;

                if (string.IsNullOrEmpty(userId))
                    userId = User.Claims.FirstOrDefault().Value;

                var user = _userManager.FindByIdAsync(userId);
                if (user == null)
                    return Unauthorized();

                return Ok(new
                {
                    UserName = claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name)?.Value,
                    UserId = claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value,
                    Role = claims.FirstOrDefault(x => x.Type == "role")?.Value, // Check role
                    Nome = claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.GivenName)?.Value, // Add other claims as needed
                    Sobrenome = claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.FamilyName)?.Value,
                    Email = claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email)?.Value
                });
            }
            catch (Exception ex) 
            {
                return StatusCode(500, ex.Message);
            }            
        }

        [HttpGet("csrf-token")]
        public IActionResult GetCsrfToken()
        {
            // ASP.NET Core auto-generates and sets the CSRF token in a cookie
            return Ok();
        }

        [HttpPost]
        [Route("{username}")]
        [AllowAnonymous]
        public async Task<IActionResult> SendResetEmail(string username)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                {
                    user = await _userManager.FindByEmailAsync(username);
                    if (user == null)
                        return NoContent();
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("no-reply@meicaixa.com.br");
                mail.To.Add(user.Email!);

                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                var isDevelopment = environment == Environments.Development;
                var url = "";
                if (isDevelopment)
                    url = $"http://localhost:5173/ResetSenha/{encodedToken}";
                else
                    url = $"https://www.meicaixa.com.br/ResetSenha/{encodedToken}";

                mail.Subject = "Recuperação de Senha";
                mail.IsBodyHtml = true;

                mail.Body = $"<body style=\"margin:0; padding:0; font-family:Arial, sans-serif; background-color:#f9f9f9;\"> " +
                    $" <table align=\"center\" width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" style=\"max-width:600px; margin:auto; background-color:#ffffff; border:1px solid #ddd;\">    <tr> " +
                    $"     <td style=\"background-color:rgb(138, 43, 226); padding:20px; text-align:center; color:white;\"> " +
                    $"       <h1 style=\"margin:0;\">Recupere sua senha</h1>      " +
                    $"</td>    " +
                    $"</tr> " +
                    $"   <tr>      " +
                    $"<td style=\"padding:30px; color:#333;\">        " +
                    $"<p>Olá,</p> " +
                    $"<p>Recebemos uma solicitação para redefinir a sua senha. Clique no botão abaixo para continuar com o processo:</p> " +
                    $"<p style=\"text-align:center; margin: 30px 0;\">" +
                    $"<a href=\"{url}\" style=\"background-color:rgb(138, 43, 226); color:white; text-decoration:none; padding:12px 24px; border-radius:5px; display:inline-block; font-weight:bold;\">Redefinir senha </a>        </p>        <p>Se você não solicitou a redefinição de senha, pode ignorar este e-mail.</p>        <p>Obrigado,<br>A equipe do MEICaixa</p>      </td>    </tr>    <tr>      <td style=\"background-color:#f1f1f1; text-align:center; padding:15px; font-size:12px; color:#777;\">        © {DateTime.Now.Year} MEICaixa. Todos os direitos reservados.      </td>    </tr>  </table></body>";

                var client = new SmtpClient("smtp.hostinger.com", 587);
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("no-reply@meicaixa.com.br", "H:de{DC7M(h&H]Pc");
                client.Send(mail);
                client.Dispose();
                mail.Dispose();

                return Ok();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return StatusCode(500, ex.InnerException.Message);

                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task SendConfirmationEmail(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(username);
                if (user == null)
                    return;
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("no-reply@meicaixa.com.br");
            mail.To.Add(user.Email!);

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var isDevelopment = environment == Environments.Development;
            var url = "";
            if (isDevelopment)
                url = $"http://localhost:5173/ConfirmarEmail/{encodedToken}";
            else
                url = $"https://www.danieloliveira.net.br/MEICaixa/ConfirmarEmail/{encodedToken}";

            mail.Subject = "Confirmação de Email";
            mail.IsBodyHtml = true;

            mail.Body = $"<body style=\"margin:0; padding:0; font-family:Arial, sans-serif; background-color:#f9f9f9;\"> " +
                $" <table align=\"center\" width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" style=\"max-width:600px; margin:auto; background-color:#ffffff; border:1px solid #ddd;\">    <tr> " +
                $"     <td style=\"background-color:rgb(138, 43, 226); padding:20px; text-align:center; color:white;\"> " +
                $"       <h1 style=\"margin:0;\">Confirme seu email</h1>      " +
                $"</td>    " +
                $"</tr> " +
                $"   <tr>      " +
                $"<td style=\"padding:30px; color:#333;\">        " +
                $"<p>Olá,</p> " +
                $"<p>Agradecemos muito por criar sua conta em nossa plataforma. Clique no botão abaixo para confirmar seu email:</p> " +
                $"<p style=\"text-align:center; margin: 30px 0;\">" +
                $"<a href=\"{url}\" style=\"background-color:rgb(138, 43, 226); color:white; text-decoration:none; padding:12px 24px; border-radius:5px; display:inline-block; font-weight:bold;\">Confirmar Email </a>        </p>        <p>Obrigado,<br>A equipe do MEICaixa</p>      </td>    </tr>    <tr>      <td style=\"background-color:#f1f1f1; text-align:center; padding:15px; font-size:12px; color:#777;\">        © {DateTime.Now.Year} MEICaixa. Todos os direitos reservados.      </td>    </tr>  </table></body>";

            var client = new SmtpClient("smtp.hostinger.com", 587);
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("no-reply@meicaixa.com.br", "H:de{DC7M(h&H]Pc");
            client.Send(mail);
            client.Dispose();
            mail.Dispose();

        }


        [HttpPut]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest("Preencha o email");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(email);
                if (user == null)
                    return NoContent();
            }
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (result.Succeeded)
                return Ok();
            return StatusCode(500, result.Errors);
        }


        [HttpPut]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetSenhaDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Email))
                return BadRequest("Preencha o email");

            if (string.IsNullOrEmpty(dto.NewPassword))
                return BadRequest("Preencha a nova senha");

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(dto.Email);
                if (user == null)
                    return NoContent();
            }
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(dto.Token));
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, dto.NewPassword);
            if (result.Succeeded)
                return Ok();
            return StatusCode(500, result.Errors);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] SignUpDTO dto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(dto.Email);
                    if (user == null)
                        return NoContent();
                }
                user.Nome = dto.Nome;
                user.Sobrenome = dto.Sobrenome;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                    return Ok();
                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(500, ex.Message);

                return StatusCode(500, ex.InnerException.Message);
            }
        }
    }
}
