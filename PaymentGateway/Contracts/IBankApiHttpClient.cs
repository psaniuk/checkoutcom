
using System.Threading.Tasks;
using checkoutcom.paymentgateway.Models.DTO;

namespace checkoutcom.paymentgateway.Contracts
{
    public interface IBankApiHttpClient
    {
        Task<Transaction> SubmitPaymentAsync(PaymentDetails payment);
    }
}
