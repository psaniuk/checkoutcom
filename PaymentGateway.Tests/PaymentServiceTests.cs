using System;
using Xunit;
using System.Threading.Tasks;
using checkoutcom.paymentgateway.Exceptons;
using checkoutcom.paymentgateway.Contracts;
using checkoutcom.paymentgateway.Services;
using checkoutcom.paymentgateway.Models.DTO;
using System.Globalization;

namespace PaymentGateway.Tests
{
    public class PaymentServiceTests
    {
        [Fact]
        public async Task Given_Empty_CardNumber_Assume_ProcessPaymentAsync_throws_PaymentValidationException()
        {
            var paymentService = CreatePaymentService(null, null);  
            var paymentDetails = CreatePaymentDetails(cardNumber: string.Empty) ;
            await Assert.ThrowsAsync<PaymentValidationException>(async () => await paymentService.ProcessPaymentAsync(paymentDetails));
        }

        [Fact]
        public async Task Given_Empty_Amount_Assume_ProcessPaymentAsync_throws_PaymentValidationException()
        {
            var paymentDetails = CreatePaymentDetails(amount: string.Empty) ;
            await Assert.ThrowsAsync<PaymentValidationException>(async () => await CreatePaymentService().ProcessPaymentAsync(paymentDetails));
        }

        [Fact]
        public async Task Given_Empty_Currency_Assume_ProcessPaymentAsync_throws_PaymentValidationException()
        {
            var paymentDetails = CreatePaymentDetails(currency: string.Empty) ;
            await Assert.ThrowsAsync<PaymentValidationException>(async () => await CreatePaymentService().ProcessPaymentAsync(paymentDetails));
        }

        [Fact]
        public async Task Given_Empty_ExpireAt_Assume_ProcessPaymentAsync_throws_PaymentValidationException()
        {
            var paymentDetails = CreatePaymentDetails(expireAt: string.Empty) ;
            await Assert.ThrowsAsync<PaymentValidationException>(async () => await CreatePaymentService().ProcessPaymentAsync(paymentDetails));
        }

        [Fact]
        public async Task Given_Empty_CVV_Assume_ProcessPaymentAsync_throws_PaymentValidationException()
        {
            var paymentDetails = CreatePaymentDetails(cvv: string.Empty) ;
            await Assert.ThrowsAsync<PaymentValidationException>(async () => await CreatePaymentService().ProcessPaymentAsync(paymentDetails));
        }

        [Fact]
        public async Task Given_12345_As_CardNumber_Assume_ProcessPaymnetAsync_Throws_PaymentValidationException()
        {
            var paymentDetails = CreatePaymentDetails(cardNumber: "12345") ;
            await Assert.ThrowsAsync<PaymentValidationException>(async () => await CreatePaymentService().ProcessPaymentAsync(paymentDetails));
        }

        [Fact]
        public async Task Given_ABC_As_CardNumber_Assume_ProcessPaymnetAsync_Throws_PaymentValidationException()
        {
            var paymentDetails = CreatePaymentDetails(cardNumber: "ABC") ;
            await Assert.ThrowsAsync<PaymentValidationException>(async () => await CreatePaymentService().ProcessPaymentAsync(paymentDetails));
        }

        [Fact]
        public async Task Given_1234123412341234_As_CardNumber_Assume_ProcessPaymnetAsync_Is_Successfull()
        {
            var paymentDetails = CreatePaymentDetails(cardNumber: "1234123412341234") ;
            var id = await CreatePaymentService().ProcessPaymentAsync(paymentDetails);
            
            Assert.True(id != Guid.Empty);
        }

        [Fact]
        public async Task Given_12z_As_Amount_Assume_ProcessPaymentAsync_Throws_PaymentValidationException()
        {
            var paymentDetails = CreatePaymentDetails(amount: "12z") ;
            await Assert.ThrowsAsync<PaymentValidationException>(async () => await CreatePaymentService().ProcessPaymentAsync(paymentDetails));                
        }

        [Fact]
        public async Task Given_12_Dot_5_As_Amount_Assume_ProcessPaymentAsync_Throws_PaymentValidationException()
        {
            var paymentDetails = CreatePaymentDetails(amount: "12.5") ;
            var id = await CreatePaymentService().ProcessPaymentAsync(paymentDetails);
            
            Assert.True(id != Guid.Empty);
        }

       [Fact]
        public async Task Given_12_Comma_5_As_Amount_Assume_ProcessPaymentAsync_Throws_PaymentValidationException()
        {
            var paymentDetails = CreatePaymentDetails(amount: "12,5") ;
            var initialCulture = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("de-DE");
            var id = await CreatePaymentService().ProcessPaymentAsync(paymentDetails);
            
            Assert.True(id != Guid.Empty);
            System.Globalization.CultureInfo.CurrentCulture = initialCulture;
        }

        [Fact]
        public async Task Given_12_01_As_ExpireAt_Assume_ProcessPaymentAsync_Throws_PaymentValidationException()
        {
            var paymentDetails = CreatePaymentDetails(expireAt: "12/01") ;        
            await Assert.ThrowsAsync<PaymentValidationException>(async () => await CreatePaymentService().ProcessPaymentAsync(paymentDetails));                        
        }

        private PaymentDetails CreatePaymentDetails(string cardNumber = "1234-1234-1234-1234", string currency = "EUR", string amount = "12.5", string cvv = "123", string expireAt = "12/23") =>
            new PaymentDetails() { CardNumber = cardNumber, Amount = amount, Currency = currency, ExpireAt = expireAt, CVV = cvv };        
        private PaymentService CreatePaymentService(IBankApiHttpClient bankApiHttpClient = null, IPaymentDetailsRepository repository = null) =>  
            new PaymentService(bankApiHttpClient, repository);
        
    }
}
