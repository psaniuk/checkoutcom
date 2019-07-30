using System;
using System.Threading.Tasks;

namespace checkoutcom.paymentgateway.Contracts
{
    public interface IPaymentService
    {
        Task<Guid> ProcessPaymentAsync(PaymentDetails payment);
    }
}