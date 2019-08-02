using System;
using System.Threading.Tasks;
using checkoutcom.paymentgateway.Models;

namespace checkoutcom.paymentgateway.Contracts
{
    public interface IPaymentDetailsRepository
    {
        Task AddAsync(Payment payment);
        Task<Payment> FindBy(Guid idempotencyKey);
    }
}