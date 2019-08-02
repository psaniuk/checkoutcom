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
            await _dbContext.AddAsync(payment);
            await _dbContext.SaveChangesAsync();
        }

        public Task<Payment> FindBy(Guid id) => _dbContext.Payments.FindAsync(id);
    }
}