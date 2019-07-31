using System;
using System.Threading.Tasks;
using checkoutcom.paymentgateway.Models.DTO;

namespace checkoutcom.paymentgateway.Contracts
{
    public interface IPaymentService
    {
        Task<Guid> ProcessPaymentAsync(PaymentDetails payment);
    }
}