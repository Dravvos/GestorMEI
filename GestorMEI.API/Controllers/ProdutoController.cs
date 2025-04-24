using GestorMEI.BLL.Services.Interfaces;
using GestorMEI.DTO;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace GestorMEI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        private readonly IProdutoService _service;

        public ProdutoController(IProdutoService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return UnprocessableEntity("Id do produto não pode ser vazio");

                var produto = await _service.GetProdutoByIdAsync(id);
                if (produto == null)
                    return NotFound();

                return Ok(produto);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(500, ex.Message);
                return StatusCode(500, ex.InnerException.Message);

            }
        }

        [HttpGet]
        [Route("[action]/{empresaId}/{nome}")]
        public async Task<IActionResult> GetProdutosByEmpresaAndNome(Guid empresaId, string nome)
        {
            try
            {
                if (empresaId == Guid.Empty)
                    return UnprocessableEntity("Id da empresa não pode ser vazio");
                if (string.IsNullOrEmpty(nome))
                    return UnprocessableEntity("Nome do produto não pode ser vazio");

                var produtos = await _service.GetProdutosAsync(empresaId);
                produtos = produtos.Where(x => x.Nome.Contains(nome)).ToList();
                if (produtos == null || produtos.Any() == false)
                    return NotFound();

                return Ok(produtos);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(500, ex.Message);

                return StatusCode(500, ex.InnerException.Message);
            }
        }

        [HttpGet]
        [Route("[action]/{empresaId}")]
        public async Task<IActionResult> GetProdutosByEmpresa(Guid empresaId)
        {
            try
            {
                if (empresaId == Guid.Empty)
                    return UnprocessableEntity("Id da empresa não pode ser vazio");

                var produtos = await _service.GetProdutosAsync(empresaId);
                if (produtos == null || produtos.Any() == false)
                    return NotFound();

                return Ok(produtos);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(500, ex.Message);

                return StatusCode(500, ex.InnerException.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduto([FromBody] ProdutoDTO produto)
        {
            try
            {
                if (produto == null)
                    return UnprocessableEntity("Produto não pode ser nulo");

                produto.UsuarioInclusao = User.FindFirstValue(JwtRegisteredClaimNames.Name)!;
                await _service.CreateProdutoAsync(produto);
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
        public async Task<IActionResult> UpdateProduto(Guid id, [FromBody] ProdutoDTO produto)
        {
            try
            {
                if (id == Guid.Empty)
                    return UnprocessableEntity("Id do produto não pode ser vazio");
                if (produto == null)
                    return UnprocessableEntity("Produto não pode ser nulo");

                produto.UsuarioAlteracao = User.FindFirstValue(JwtRegisteredClaimNames.Name);
                await _service.UpdateProdutoAsync(produto);
                return Ok();
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(500, ex.Message);
                return StatusCode(500, ex.InnerException.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduto(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return UnprocessableEntity("Id do produto não pode ser vazio");
                await _service.DeleteProdutoAsync(id);
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
