using System;
using System.Threading.Tasks;
using checkoutcom.paymentgateway.Contracts;
using checkoutcom.paymentgateway.Models;
using checkoutcom.paymentgateway.Models.DTO;

namespace checkoutcom.paymentgateway.Services
{
    public class BankApiHttpClientMock : IBankApiHttpClient
    {
        public Task<Transaction> SubmitPaymentAsync(PaymentDetails payment)
        {
           return Task.FromResult(new Transaction(){ Id = Guid.NewGuid(), Status = (int) PaymentStatus.Completed });
        }
    }
}