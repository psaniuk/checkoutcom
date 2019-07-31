
using System.Threading.Tasks;
using checkoutcom.paymentgateway.Models;

namespace checkoutcom.paymentgateway.Contracts
{
    public interface IBankApiHttpClient
    {
        Task<Transaction> ProcessPaymentAsync(PaymentDetails payment);
    }
}
