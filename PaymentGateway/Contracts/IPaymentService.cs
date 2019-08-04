using System;
using System.Threading.Tasks;
using checkoutcom.paymentgateway.Models;
using checkoutcom.paymentgateway.Models.DTO;

namespace checkoutcom.paymentgateway.Contracts
{
    public interface IPaymentService
    {
        Task<Payment> FindAsync(Guid id);
        Task<Guid> ProcessPaymentAsync(PaymentDetails payment);
    }
}