using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using checkoutcom.paymentgateway.Contracts;
using checkoutcom.paymentgateway.Exceptons;
using checkoutcom.paymentgateway.Models;
using checkoutcom.paymentgateway.Models.DTO;
using Microsoft.Extensions.Primitives;

namespace checkoutcom.paymentgateway.Controllers
{
    [ApiController]
    public class PaymentGatewayController : ControllerBase
    {
        private const string IdempotencyKeyHeader = "Idempotency-Key";
        private readonly IPaymentService _paymentService;
        private readonly IIdempotencyKeyRepository _idempotencyRepository;
        private readonly ILogger<PaymentGatewayController> _logger;

        public PaymentGatewayController(IPaymentService paymentService, IIdempotencyKeyRepository idempotencyKeyRepository, ILogger<PaymentGatewayController> logger)
        {
            _paymentService = paymentService;
            _idempotencyRepository = idempotencyKeyRepository;
            _logger = logger;
        }
        
        [HttpGet]
        [Route("health")]
        public ActionResult Health() => Ok("Alive");

        [HttpGet]
        [Route("payments/{id}")]
        public async Task<ActionResult> Get(string id)
        { 
            try
            {
                if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out Guid paymentId))
                    return BadRequest("ID is not valid");

                var payment = await _paymentService.FindAsync(paymentId);
                if (payment == null)
                    return NotFound();

                return Ok(new { cardNumber = payment.CardNumber, expiryAt = payment.ExpireAt, status = (int)payment.Status });    
            }
            catch(Exception exception)
            {
                _logger.Log(LogLevel.Critical, exception, "An unhandled error has occured while GET a payment");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("payments")]
        public async Task<ActionResult> Post(PaymentDetails paymentDetails)
        {
            try
            {
                if (!ParseIdempotencyKey(out Guid idempotencyId))
                    return BadRequest($"{IdempotencyKeyHeader} header is not valid");

                var idempotencyKey = await _idempotencyRepository.FindAsync(idempotencyId);
                if (idempotencyKey != null)
                    return Ok(new { id = idempotencyKey.PaymentId});    

                Payment payment = await _paymentService.ProcessPaymentAsync(paymentDetails);
                await _idempotencyRepository.AddAsync(new IdempotencyKey(idempotencyId, payment.Id));

                return CreatedAtAction("post", new { id = payment.Id, statusCode = (int)payment.Status });
            }
            catch (PaymentValidationException validationException)
            {
                return BadRequest(validationException.Message);
            }
            catch(Exception exception)
            {
                _logger.Log(LogLevel.Critical, exception, "Post payment unhandled exception");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private bool ParseIdempotencyKey(out Guid key)
        {
            key = Guid.Empty;

            if (!Request.Headers.TryGetValue(IdempotencyKeyHeader, out StringValues idempotencyKey))
                return false;

            string keyValue = idempotencyKey.ToArray().FirstOrDefault();
            return Guid.TryParse(keyValue, out key);
        }
    }
}
