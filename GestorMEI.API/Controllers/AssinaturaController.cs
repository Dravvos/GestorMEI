using GestorMEI.BLL.Services.Interfaces;
using GestorMEI.DTO;
using MercadoPago.Client;
using MercadoPago.Client.Common;
using MercadoPago.Client.Payment;
using MercadoPago.Client.Preapproval;
using MercadoPago.Config;
using MercadoPago.Resource.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GestorMEI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AssinaturaController : ControllerBase
    {
        private readonly IAssinaturaService _service;
        private readonly ITabelaGeralItemService _tabelaGeralItemService;
        private readonly ITabelaGeralService _tabelaGeralService;

        public AssinaturaController(IAssinaturaService service, ITabelaGeralItemService tabelaGeralItemService, ITabelaGeralService tabelaGeralService)
        {
            _service = service;
            _tabelaGeralItemService = tabelaGeralItemService;
            _tabelaGeralService = tabelaGeralService;
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetKey()
        {
            try
            {
                string publicKey = "TEST-03cea708-3e86-4543-bfa4-cb30b6887939";
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                var isDevelopment = environment == Environments.Development;

                if (isDevelopment == false)
                    publicKey = "APP_USR-c8512e2c-85b7-4970-a5cf-999a7841049b";

                var encodedKey = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(publicKey));
                return Ok(encodedKey);

            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(500, ex.Message);
                return StatusCode(500, ex.InnerException.Message);

            }
        }

        [HttpGet]
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
                var assinatura = await _service.GetAssinaturaByUserId(usuarioId);
                if (assinatura == null)
                    return Ok();

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
                return StatusCode(201, assinatura.Id);
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
        public async Task<IActionResult> DeleteAssinatura(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return UnprocessableEntity("Id da assinatura não pode ser vazio");
                await _service.DeleteAssinatura(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    return StatusCode(500, ex.Message);
                return StatusCode(500, ex.InnerException.Message);
            }
        }

        [HttpPost("Process/{tipoAssinaturaId}")]
        public async Task<IActionResult> ProcessarPagamento([FromBody] MercadoPagoDTO cardForm, Guid tipoAssinaturaId)
        {
            if (cardForm == null || cardForm.FormData == null)
                return UnprocessableEntity("Dados do pagamento inválidos");
            MercadoPagoConfig.AccessToken = "TEST-7537308538793161-041922-98035968fe22dd4906d91066126786e7-706381060";

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var isDevelopment = environment == Environments.Development;

            if (isDevelopment == false)
                MercadoPagoConfig.AccessToken = Environment.GetEnvironmentVariable("MercadoPagoSecretKey");

            try
            {
                var requestOptions = new RequestOptions();

                HttpContext.Request.Cookies.TryGetValue("AuthToken", out var cookie);

                if (string.IsNullOrEmpty(cookie))
                    return Unauthorized();

                var decodedToken = new JwtSecurityTokenHandler().ReadJwtToken(cookie);
                var claims = decodedToken.Claims;

                var jti = claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;

                requestOptions.CustomHeaders.Add("x-idempotency-key", jti);
                var paymentRequest = new PaymentCreateRequest();

                var tipoAssinatura = await _tabelaGeralItemService.GetByIdAsync(tipoAssinaturaId);
                if (tipoAssinatura.Sigla == "ESS")
                    cardForm.FormData.Transaction_Amount = 49M;
                else if (tipoAssinatura.Sigla == "AVNC")
                    cardForm.FormData.Transaction_Amount = 99M;
                else if(tipoAssinatura.Sigla == "PRO")
                    cardForm.FormData.Transaction_Amount = 179M;
                else
                    return UnprocessableEntity("Tipo de assinatura inválida");

                if (cardForm.PaymentType == "bank_transfer")
                {
                    paymentRequest.TransactionAmount = cardForm.FormData.Transaction_Amount;
                    paymentRequest.Description = "Sistema de Gestão para MEI";
                    paymentRequest.PaymentMethodId = "pix";
                    paymentRequest.Payer = new PaymentPayerRequest
                    {
                        Email = cardForm.FormData.Payer.Email,
                        FirstName = claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.GivenName)?.Value,
                        LastName = claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.FamilyName)?.Value,
                    };
                }
                else
                {
                    paymentRequest.TransactionAmount = cardForm.FormData.Transaction_Amount;
                    paymentRequest.Token = cardForm.FormData.Token;
                    paymentRequest.Description = "Sistema de Gestão para MEI";
                    paymentRequest.Installments = cardForm.FormData.Installments;
                    paymentRequest.PaymentMethodId = cardForm.FormData.Payment_Method_Id;

                    paymentRequest.Payer = new PaymentPayerRequest
                    {
                        Email = cardForm.FormData.Payer.Email,
                    };
                }

                var client = new PaymentClient();

                Payment payment = await client.CreateAsync(paymentRequest, requestOptions);
                if (payment.Status.ToUpper().Trim() == "APPROVED")
                    payment = await client.CaptureAsync(payment.Id ?? 0);
                else if (payment.Status.ToUpper().Trim() == "PENDING")
                    return Ok(payment);

                if (payment.Status.ToUpper().Trim() == "APPROVED")
                {
                    var usuarioId = Guid.Parse(claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value);
                    var assinaturaExistente = await _service.GetAssinaturaByUserId(usuarioId);

                    var tgStatusAssinatura = await _tabelaGeralService.GetByNomeAsync("StatusAssinatura");
                    var statusAssinatura = await _tabelaGeralItemService.GetBySiglaAsync(tgStatusAssinatura.Id.GetValueOrDefault(), "ATV");
                    if (assinaturaExistente == null)
                    {
                        await _service.CreateAssinatura(new AssinaturaDTO
                        {
                            DataInicio = DateTime.UtcNow.Date,
                            DataFim = DateTime.UtcNow.Date.AddDays(30),
                            UsuarioId = usuarioId,
                            IdTGTipoAssinatura = tipoAssinaturaId,
                            IdTGStatusAssinatura = statusAssinatura.Id.GetValueOrDefault(),
                            UsuarioInclusao = User.FindFirstValue(JwtRegisteredClaimNames.Name)
                        });
                    }
                    else
                    {
                        assinaturaExistente.IdTGStatusAssinatura = statusAssinatura.Id.GetValueOrDefault();
                        assinaturaExistente.IdTGTipoAssinatura = tipoAssinaturaId;
                        assinaturaExistente.DataInicio = DateTime.UtcNow.Date.ToUniversalTime();
                        assinaturaExistente.DataFim = DateTime.UtcNow.Date.AddDays(30);
                        assinaturaExistente.UsuarioAlteracao = User.FindFirstValue(JwtRegisteredClaimNames.Name);
                        await _service.UpdateAssinatura(assinaturaExistente);

                    }
                }
                return Ok(payment.Status);
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