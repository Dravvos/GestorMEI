using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GestorMEI.BLL.Services.Interfaces;
using GestorMEI.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GestorMEI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstoqueController : ControllerBase
    {
        private readonly IEstoqueService _service;

        public EstoqueController(IEstoqueService service)
        {
            _service = service;
        }

        [HttpGet("{empresaId}")]
        public async Task<IActionResult> GetEstoque(Guid empresaId)
        {
            try
            {
                if (empresaId == Guid.Empty)
                    return UnprocessableEntity("Id da empresa não pode ser vazio");
                var estoque = await _service.GetEstoqueAsync(empresaId);
                if (estoque == null || estoque.Any() == false)
                    return NotFound();
                return Ok(estoque);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(500, ex.Message);
                return StatusCode(500, ex.InnerException.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateEstoque([FromBody] EstoqueDTO estoque)
        {
            try
            {
                if (estoque == null)
                    return UnprocessableEntity("Estoque não pode ser nulo");
                estoque.UsuarioInclusao = User.FindFirstValue(JwtRegisteredClaimNames.Name)!;
                await _service.AddToEstoqueAsync(estoque);
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
        public async Task<IActionResult> UpdateEstoque(Guid id, [FromBody] EstoqueDTO estoque)
        {
            try
            {
                if (estoque == null)
                    return UnprocessableEntity("Estoque não pode ser nulo");
                if (id == Guid.Empty)
                    return UnprocessableEntity("Id do estoque não pode ser vazio");
                if (id != estoque.Id)
                    return UnprocessableEntity("Id do estoque não confere com o Id do objeto");

                estoque.UsuarioAlteracao = User.FindFirstValue(JwtRegisteredClaimNames.Name);
                await _service.UpdateEstoqueAsync(estoque);
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
        public async Task<IActionResult> DeleteEstoque(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return UnprocessableEntity("Id do estoque não pode ser vazio");
                await _service.DeleteEstoqueAsync(id);
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
