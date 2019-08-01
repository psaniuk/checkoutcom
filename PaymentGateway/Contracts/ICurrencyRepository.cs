using System.Threading.Tasks;
using checkoutcom.paymentgateway.Models;

namespace checkoutcom.paymentgateway.Contracts
{
    public interface ICurrencyRepository
    {
        Task<Currency> FindBy(string code);
    }
}