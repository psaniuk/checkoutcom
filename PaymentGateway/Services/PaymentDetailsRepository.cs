using System;
using System.Threading.Tasks;
using checkoutcom.paymentgateway.Contracts;
using checkoutcom.paymentgateway.Models;

namespace checkoutcom.paymentgateway.Services
{
    public class PaymentDetailsRepository : IPaymentDetailsRepository
    {
        public Task AddAsync(Payment payment)
        {
            throw new NotImplementedException();
        }

        public Task<Payment> FindBy(Guid idempotencyKey)
        {
            throw new NotImplementedException();
        }

        public Task<Payment> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}