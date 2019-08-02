using System;
using System.Threading.Tasks;
using checkoutcom.paymentgateway.Contracts;
using checkoutcom.paymentgateway.Models.DTO;

namespace checkoutcom.paymentgateway.Services
{
    public class BankApiHttpClientMock : IBankApiHttpClient
    {
        public Task<BankTransaction> SubmitPaymentAsync(PaymentDetails payment)
        {
           return Task.FromResult(new BankTransaction(){ Id = Guid.NewGuid(), Status = TransactionStatus.Success });
        }
    }
}