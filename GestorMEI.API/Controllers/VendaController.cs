using GestorMEI.BLL.Services.Interfaces;
using GestorMEI.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace GestorMEI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VendaController : ControllerBase
    {
        private readonly IVendaService _vendaService;
        private readonly IAssinaturaService _assinaturaService;

        public VendaController(IVendaService vendaService, IAssinaturaService assinaturaService)
        {
            _vendaService = vendaService;
            _assinaturaService = assinaturaService;
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
                return StatusCode(201, venda.Id);
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
                
                HttpContext.Request.Cookies.TryGetValue("AuthToken", out var cookie);

                if (string.IsNullOrEmpty(cookie))
                    return Unauthorized();

                var decodedToken = new JwtSecurityTokenHandler().ReadJwtToken(cookie);
                var claims = decodedToken.Claims;

                var usuarioId = Guid.Parse(claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value);

                var vendas = await _vendaService.GetVendasAsync(empresaId);
                long objectSize = ObterEspacoUtilizado(vendas);

                var assinatura = await _assinaturaService.GetAssinaturaByUserId(usuarioId);
                if ((assinatura.TipoAssinatura.Sigla == "ESS" && objectSize > 1024 * 1024 * 80)
                    || (assinatura.TipoAssinatura.Sigla == "AVNC" && objectSize > 1024 * 1024 * 200)
                    || (assinatura.TipoAssinatura.Sigla == "PRO" && objectSize > 1024 * 1024 * 600))
                {
                    return StatusCode(403, "A quantidade de dados da sua assinatura chegou ao limite. Atualize seu plano");
                }

                if (Data != null)
                    vendas = vendas.Where(x => x.DataVenda == Data);

                if (vendas == null || vendas.Any() == false)
                    return Ok();
                return Ok(vendas);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(500, ex.Message);
                return StatusCode(500, ex.InnerException.Message);
            }
        }

        private long ObterEspacoUtilizado(IEnumerable<VendaDTO> vendas)
        {
            long objectSize = 0;
            foreach (var venda in vendas)
            {
                objectSize += Marshal.SizeOf(typeof(Guid));//Id
                objectSize += Marshal.SizeOf(typeof(Guid));//EmpresaId
                objectSize += Marshal.SizeOf(typeof(float)); // Valor Venda
                objectSize += Marshal.SizeOf(typeof(bool)); //Com NF
                objectSize += Marshal.SizeOf(typeof(DateOnly)); // Data Venda
                objectSize += Marshal.SizeOf(typeof(Guid));//TG TipoVenda
            }

            //Empresa
            objectSize += Marshal.SizeOf(typeof(Guid));//Id
            objectSize += vendas.FirstOrDefault()?.Empresa?.RazaoSocial?.Length * Marshal.SizeOf(typeof(char)) ?? 0;
            objectSize += vendas.FirstOrDefault()?.Empresa?.NomeFantasia?.Length * Marshal.SizeOf(typeof(char)) ?? 0;
            objectSize += vendas.FirstOrDefault()?.Empresa?.CNPJ?.Length * Marshal.SizeOf(typeof(char)) ?? 0;
            objectSize += vendas.FirstOrDefault()?.Empresa?.Telefone?.Length * Marshal.SizeOf(typeof(char)) ?? 0;
            objectSize += vendas.FirstOrDefault()?.Empresa?.Email?.Length * Marshal.SizeOf(typeof(char)) ?? 0;
            objectSize += vendas.FirstOrDefault()?.Empresa?.CEP?.Length * Marshal.SizeOf(typeof(char)) ?? 0;
            objectSize += vendas.FirstOrDefault()?.Empresa?.Endereco?.Length * Marshal.SizeOf(typeof(char)) ?? 0;
            objectSize += vendas.FirstOrDefault()?.Empresa?.Complemento?.Length * Marshal.SizeOf(typeof(char)) ?? 0;
            objectSize += vendas.FirstOrDefault()?.Empresa?.Bairro?.Length * Marshal.SizeOf(typeof(char)) ?? 0;
            objectSize += vendas.FirstOrDefault()?.Empresa?.Cidade?.Length * Marshal.SizeOf(typeof(char)) ?? 0;
            objectSize += vendas.FirstOrDefault()?.Empresa?.Estado?.Length * Marshal.SizeOf(typeof(char)) ?? 0;
            objectSize += Marshal.SizeOf(typeof(uint));
            return objectSize;
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
                    return Ok();
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
                return NoContent();
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
                return NoContent();
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
