using System;
using System.Threading.Tasks;
using checkoutcom.paymentgateway.Contracts;
using checkoutcom.paymentgateway.Exceptons;

namespace checkoutcom.paymentgateway.Services
{
    public class PaymentService : IPaymentService
    {
        public Task<Guid> ProcessPaymentAsync(PaymentDetails payment)
        {
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            if (string.IsNullOrWhiteSpace(payment.CardNumber))
                throw new PaymentValidationException("Card number is required");
            
            if (string.IsNullOrWhiteSpace(payment.Currency))
                throw new PaymentValidationException("Currency is required");

            throw new NotImplementedException();
        }
    }
}