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
using Microsoft.IdentityModel.Tokens;
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

        [HttpPost("Process")]
        public async Task<IActionResult> ProcessarPagamento([FromBody] MercadoPagoDTO cardForm)
        {
            MercadoPagoConfig.AccessToken = "TEST-7537308538793161-041922-98035968fe22dd4906d91066126786e7-706381060";
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
                var paymentRequest = new PaymentCreateRequest
                {
                    TransactionAmount = cardForm.FormData.Transaction_Amount,
                    Token = cardForm.FormData.Token,
                    Description = "Sistema de Gestão para MEI",
                    Installments = cardForm.FormData.Installments,
                    PaymentMethodId = cardForm.FormData.Payment_Method_Id,

                    Payer = new PaymentPayerRequest
                    {
                        Email = cardForm.FormData.Payer.Email,
                        Identification = new IdentificationRequest
                        {
                            Type = cardForm.FormData.Payer.Identification.Type,
                            Number = cardForm.FormData.Payer.Identification.Number,
                        },
                    },
                };

                var client = new PaymentClient();
                var c = new MercadoPago.Client.Preapproval.PreapprovalClient();

                Payment payment = await client.CreateAsync(paymentRequest, requestOptions);
                if (payment.Status.ToUpper().Trim() == "APPROVED")
                    payment = await client.CaptureAsync(payment.Id ?? 0);
                else if (payment.Status.ToUpper().Trim() == "REJECTED")
                {
                    return StatusCode(500, "Pagamento não pode ser processado");
                }
                if (payment.Status.ToUpper().Trim() == "APPROVED")
                {
                    var assinatura = new PreapprovalCreateRequest();
                    assinatura.AutoRecurring = new PreApprovalAutoRecurringCreateRequest
                    {
                        CurrencyId = "BRL",
                        Frequency = 1,
                        FrequencyType = "months",
                        TransactionAmount = cardForm.FormData.Transaction_Amount,
                    };
                    assinatura.BackUrl = "http://127.0.0.1:5173/Sucesso";
                    assinatura.Reason = "Sistema de Gestão para MEI";
                    assinatura.PayerEmail = cardForm.FormData.Payer.Email;
                    
                    var assinaturaClient = new PreapprovalClient();
                    var ass = await assinaturaClient.CreateAsync(assinatura);
                    
                    var usuarioId = Guid.Parse(claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value);
                    var assinaturaExistente = await _service.GetAssinaturaByUserId(usuarioId);
                    
                    if (assinaturaExistente == null)
                    {
                        await _service.CreateAssinatura(new AssinaturaDTO
                        {
                            DataInicio = DateTime.UtcNow.Date.ToUniversalTime(),
                            DataFim = DateTime.UtcNow.Date.AddDays(30),
                            UsuarioId = usuarioId,

                        });
                    }
                    else
                    {

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