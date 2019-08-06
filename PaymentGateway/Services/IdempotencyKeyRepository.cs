using System;
using System.Threading.Tasks;
using checkoutcom.paymentgateway.Contracts;
using checkoutcom.paymentgateway.Models;

namespace checkoutcom.paymentgateway.Services
{
    public class IdempotencyKeyRepository: IIdempotencyKeyRepository
    {
        private readonly PaymentGatewayDbContext _dbContext;
        
        public IdempotencyKeyRepository(PaymentGatewayDbContext dbContext) => _dbContext = dbContext;

        public async Task AddAsync(IdempotencyKey key)
        {
            if (key == null)
                throw new ArgumentNullException();

            await _dbContext.AddAsync(key);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IdempotencyKey> FindAsync(Guid key)
        {
            if (key == Guid.Empty)
                throw new ArgumentException("Key is required");
                 
            return await _dbContext.IdempotencyKeys.FindAsync(key);
        }
    }
}