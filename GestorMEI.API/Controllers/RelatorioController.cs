using GestorMEI.BLL.Services.Interfaces;
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
    public class RelatorioController : ControllerBase
    {
        private readonly IRelatorioService _relatorioService;
        private readonly IEmpresaService _empresaService;
        private readonly IVendaService _vendaService;

        public RelatorioController(IRelatorioService relatorioService, IEmpresaService empresaService, IVendaService vendaService)
        {
            _relatorioService = relatorioService;
            _empresaService = empresaService;
            _vendaService = vendaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRelatorioVendas(DateOnly? dataInicio, DateOnly? dataFim)
        {
            try
            {
                HttpContext.Request.Cookies.TryGetValue("AuthToken", out var cookie);

                if (string.IsNullOrEmpty(cookie))
                    return Unauthorized();

                var decodedToken = new JwtSecurityTokenHandler().ReadJwtToken(cookie);
                var claims = decodedToken.Claims;

                var usuarioId = Guid.Parse(claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value);
                var empresaId = await _empresaService.GetEmpresaIdByUserIdAsync(usuarioId);
                var relatorio = await _relatorioService.GerarRelatorioVendas(empresaId, dataInicio, dataFim);
                if (relatorio == null || relatorio.Count == 0)
                    return Ok();

                return Ok(relatorio);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return StatusCode(500, ex.InnerException.Message);

                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetGraficoVendas(DateOnly? dataInicio, DateOnly? dataFim)
        {
            try
            {
                HttpContext.Request.Cookies.TryGetValue("AuthToken", out var cookie);

                if (string.IsNullOrEmpty(cookie))
                    return Unauthorized();

                var decodedToken = new JwtSecurityTokenHandler().ReadJwtToken(cookie);
                var claims = decodedToken.Claims;

                var usuarioId = Guid.Parse(claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value);
                var empresaId = await _empresaService.GetEmpresaIdByUserIdAsync(usuarioId);
                var vendas = await _vendaService.GetVendasByDateAsync(empresaId, dataInicio, dataFim);
                if (vendas == null || vendas.Any() == false)
                    return NotFound();

                return Ok(vendas);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return StatusCode(500, ex.InnerException.Message);

                return StatusCode(500, ex.Message);
            }
        }
    }
}
