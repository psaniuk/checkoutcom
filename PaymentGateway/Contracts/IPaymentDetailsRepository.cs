using System;
using System.Threading.Tasks;
using checkoutcom.paymentgateway.Models;

namespace checkoutcom.paymentgateway.Contracts
{
    public interface IPaymentDetailsRepository
    {
        Task AddAsync(Payment payment);
        Task<Payment> GetAsync(Guid id);
        Task<Payment> FindBy(Guid idempotencyKey);
    }
}