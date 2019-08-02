using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using checkoutcom.paymentgateway.Contracts;
using checkoutcom.paymentgateway.Exceptons;
using checkoutcom.paymentgateway.Models.DTO;

namespace checkoutcom.paymentgateway.Controllers
{
    [ApiController]
    public class PaymentGatewayController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentGatewayController> _logger;

        public PaymentGatewayController(IPaymentService paymentService, ILogger<PaymentGatewayController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }
        
        [HttpGet]
        [Route("health")]
        public ActionResult Health() => Ok("Alive");

        [HttpPost]
        [Route("payments")]
        public async Task<ActionResult> Post(PaymentDetails paymentDetails)
        {
            try
            {
                Guid paymentId = await _paymentService.ProcessPaymentAsync(paymentDetails);
                return CreatedAtAction("post", new { id = paymentId});
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
    }
}
