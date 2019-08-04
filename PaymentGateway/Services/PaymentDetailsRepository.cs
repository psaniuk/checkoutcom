using System;
using System.Threading.Tasks;
using checkoutcom.paymentgateway.Contracts;
using checkoutcom.paymentgateway.Models;

namespace checkoutcom.paymentgateway.Services
{
    public class PaymentDetailsRepository : IPaymentDetailsRepository
    {
        private PaymentGatewayDbContext _dbContext;

        public PaymentDetailsRepository(PaymentGatewayDbContext dbContext) => _dbContext = dbContext;
        public async Task AddAsync(Payment payment)
        {
            if (payment == null)
                throw new ArgumentNullException();

            await _dbContext.AddAsync(payment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Payment> FindAsync(Guid id) 
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id is empty");

            return await _dbContext.Payments.FindAsync(id);
        } 
    }
}