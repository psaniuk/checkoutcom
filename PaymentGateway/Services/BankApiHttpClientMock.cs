using System.Threading.Tasks;
using checkoutcom.paymentgateway.Contracts;
using checkoutcom.paymentgateway.Models.DTO;

namespace checkoutcom.paymentgateway.Services
{
    public class BankApiHttpClientMock : IBankApiHttpClient
    {
        public Task<BankTransaction> SubmitPaymentAsync(PaymentDetails payment)
        {
            throw new System.NotImplementedException();
        }
    }
}