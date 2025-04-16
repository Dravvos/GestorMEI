using GestorMEI.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace GestorMEI.Identity.Services
{
    public interface ITokenService
    {
        Task<string> GenerateTokenAsync(ApplicationUser user, UserManager<ApplicationUser> userManager);
        void SetTokensInsideCookie(string token, HttpContext context);
    }
}
