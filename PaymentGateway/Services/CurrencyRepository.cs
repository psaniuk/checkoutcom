using System.Threading.Tasks;
using checkoutcom.paymentgateway.Contracts;
using checkoutcom.paymentgateway.Models;

namespace checkoutcom.paymentgateway.Services
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly PaymentGatewayDbContext _dbContext;

        public CurrencyRepository(PaymentGatewayDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Currency currency)
        {
            await _dbContext.Currencies.AddAsync(currency);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<Currency> FindBy(string code)
        {
            return await _dbContext.Currencies.FindAsync(code);
        }
    }
}