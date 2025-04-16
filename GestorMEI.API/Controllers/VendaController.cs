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
    public class VendaController : ControllerBase
    {
        private readonly IVendaService _vendaService;

        public VendaController(IVendaService vendaService)
        {
            _vendaService = vendaService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateVenda([FromBody] VendaDTO venda)
        {
            try
            {
                venda.UsuarioInclusao = User.FindFirstValue(JwtRegisteredClaimNames.Name);
                if (venda == null)
                    return UnprocessableEntity("Venda não pode ser nula");
                await _vendaService.CreateVendaAsync(venda);
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

        [HttpGet("{empresaId}")]
        public async Task<IActionResult> GetVendas(Guid empresaId, DateOnly? Data = null)
        {
            try
            {
                if (empresaId == Guid.Empty)
                    return UnprocessableEntity("Id da empresa não pode ser vazio");
                var vendas = await _vendaService.GetVendasAsync(empresaId);

                if(Data != null)
                    vendas = vendas.Where(x => x.DataVenda == Data).ToList();

                if (vendas == null || vendas.Any() == false)
                    return NotFound();
                return Ok(vendas);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(500, ex.Message);
                return StatusCode(500, ex.InnerException.Message);
            }
        }

        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<IActionResult> GetVendaById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return UnprocessableEntity("Id da venda não pode ser vazio");
                var venda = await _vendaService.GetVendaByIdAsync(id);
                if (venda == null)
                    return NotFound();
                return Ok(venda);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(500, ex.Message);
                return StatusCode(500, ex.InnerException.Message);
            }
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVenda(Guid id, [FromBody] VendaDTO venda)
        {
            try
            {
                if (id == Guid.Empty)
                    return UnprocessableEntity("Id da venda não pode ser vazio");
                if (venda == null)
                    return UnprocessableEntity("Venda não pode ser nula");
                if (id != venda.Id)
                    return UnprocessableEntity("Id da venda não confere com o Id do objeto");
                
                venda.UsuarioAlteracao = User.FindFirstValue(JwtRegisteredClaimNames.Name);

                await _vendaService.UpdateVendaAsync(venda);
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
        public async Task<IActionResult> DeleteVenda(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return UnprocessableEntity("Id da venda não pode ser vazio");
                await _vendaService.DeleteVendaAsync(id);
                return Ok();
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
