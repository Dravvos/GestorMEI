using GestorMEI.BLL.Services.Interfaces;
using GestorMEI.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GestorMEI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AssinaturaController : ControllerBase
    {
        private readonly IAssinaturaService _service;
        public AssinaturaController(IAssinaturaService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetById()
        {
            try
            {
                HttpContext.Request.Cookies.TryGetValue("AuthToken", out var cookie);

                if (string.IsNullOrEmpty(cookie))
                    return Unauthorized();

                var decodedToken = new JwtSecurityTokenHandler().ReadJwtToken(cookie);
                var claims = decodedToken.Claims;

                var usuarioId = Guid.Parse(claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value);
                if (usuarioId == Guid.Empty)
                    return UnprocessableEntity("Id do usuário não pode ser vazio");
                var assinatura = await _service.GetAssinaturaByUserId(usuarioId);
                if (assinatura == null)
                    return NotFound();

                return Ok(assinatura);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(500, ex.Message);
                return StatusCode(500, ex.InnerException.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAssinatura([FromBody] AssinaturaDTO assinatura)
        {
            try
            {
                assinatura.UsuarioInclusao = User.FindFirstValue(JwtRegisteredClaimNames.Name);

                if (assinatura == null)
                    return UnprocessableEntity("Assinatura não pode ser nula");
                await _service.CreateAssinatura(assinatura);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(500, ex.Message);
                return StatusCode(500, ex.InnerException.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAssinatura(Guid id, [FromBody] AssinaturaDTO assinatura)
        {
            try
            {
                assinatura.UsuarioAlteracao = User.FindFirstValue(JwtRegisteredClaimNames.Name);
                if (id == Guid.Empty)
                    return UnprocessableEntity("Id da assinatura não pode ser vazio");
                if (assinatura == null)
                    return UnprocessableEntity("Assinatura não pode ser nula");
                await _service.UpdateAssinatura(assinatura);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(500, ex.Message);
                return StatusCode(500, ex.InnerException.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssinatura(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return UnprocessableEntity("Id da assinatura não pode ser vazio");
                await _service.DeleteAssinatura(id);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
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