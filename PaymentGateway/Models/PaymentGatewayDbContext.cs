using Microsoft.EntityFrameworkCore;

namespace checkoutcom.paymentgateway.Models
{
    public class PaymentGatewayDbContext: DbContext
    {
        public PaymentGatewayDbContext(DbContextOptions<PaymentGatewayDbContext> options): base(options)
        {

        }

        public DbSet<Payment> Payments { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<IdempotencyKey> IdempotencyKeys { get; set; }
    }
}