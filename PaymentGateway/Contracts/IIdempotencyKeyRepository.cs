using System;
using System.Threading.Tasks;
using checkoutcom.paymentgateway.Models;

namespace checkoutcom.paymentgateway.Contracts
{
    public interface IIdempotencyKeyRepository
    {
        Task AddAsync(IdempotencyKey key);
        Task<IdempotencyKey> FindAsync(Guid key);
    }
}
