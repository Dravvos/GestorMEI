using GestorMEI.BLL.Services;
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
    public class EmpresaController : ControllerBase
    {
        private readonly IEmpresaService _service;

        public EmpresaController(IEmpresaService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return UnprocessableEntity("Id da empresa não pode ser vazio");

                var empresa = _service.GetEmpresaByIdAsync(id);
                if (empresa == null)
                    return Ok();

                return Ok(empresa);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(500, ex.Message);
                return StatusCode(500, ex.InnerException.Message);

            }
        }

        [HttpGet("[action]/{cnpj}")]
        public IActionResult GetByCnpj(string cnpj)
        {
            try
            {
                if (string.IsNullOrEmpty(cnpj))
                    return UnprocessableEntity("CNPJ da empresa não pode ser vazio");

                var empresa = _service.GetEmpresaByCNPJAsync(cnpj);
                if (empresa == null)
                    return Ok();

                return Ok(empresa);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(500, ex.Message);
                return StatusCode(500, ex.InnerException.Message);

            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetByUserId()
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

                var empresa = await _service.GetEmpresaByUserIdAsync(usuarioId);
                if (empresa == null)
                    return Ok();

                return Ok(empresa);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(500, ex.Message);
                return StatusCode(500, ex.InnerException.Message);

            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmpresa([FromBody] EmpresaDTO empresa)
        {
            try
            {
                empresa.UsuarioInclusao = User.FindFirstValue(JwtRegisteredClaimNames.Name);
                if (empresa == null)
                    return UnprocessableEntity("Empresa não pode ser nula");
                await _service.CreateEmpresaAsync(empresa);
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
        public async Task<IActionResult> UpdateEmpresa(Guid id, [FromBody] EmpresaDTO empresa)
        {
            try
            {
                empresa.UsuarioAlteracao = User.FindFirstValue(JwtRegisteredClaimNames.Name);
                if (empresa == null)
                    return UnprocessableEntity("Empresa não pode ser nula");
                await _service.UpdateEmpresaAsync(empresa);
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
        public async Task<IActionResult> DeleteEmpresa(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return UnprocessableEntity("Id da empresa não pode ser vazio");
                await _service.DeleteEmpresaAsync(id);
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
    }
}
