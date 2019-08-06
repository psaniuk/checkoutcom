using System;
using Xunit;
using Moq;
using System.Threading.Tasks;
using checkoutcom.paymentgateway.Exceptons;
using checkoutcom.paymentgateway.Contracts;
using checkoutcom.paymentgateway.Services;
using checkoutcom.paymentgateway.Models;
using checkoutcom.paymentgateway.Models.DTO;

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
        public async Task Given_Zero_As_Amount_Assume_ProcessPaymentAsync_throws_PaymentValidationException()
        {
            var paymentDetails = CreatePaymentDetails(amount: 0) ;
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
            var payment = await CreatePaymentService().ProcessPaymentAsync(paymentDetails);
            
            Assert.NotNull(payment);
        }

        [Fact]
        public async Task Given_Negative_Amount_Assume_ProcessPaymentAsync_Throws_PaymentValidationException()
        {
            var paymentDetails = CreatePaymentDetails(amount: -54) ;
            await Assert.ThrowsAsync<PaymentValidationException>(async () => await CreatePaymentService().ProcessPaymentAsync(paymentDetails));                
        }

        [Fact]
        public async Task Given_12_01_As_ExpireAt_Assume_ProcessPaymentAsync_Throws_PaymentValidationException()
        {
            var paymentDetails = CreatePaymentDetails(expireAt: "12/01") ;        
            await Assert.ThrowsAsync<PaymentValidationException>(async () => await CreatePaymentService().ProcessPaymentAsync(paymentDetails));                        
        }

        [Fact]
        public async Task Given_RUB_As_Currency_Assume_ProcessPaymentAsync_Throws_PaymentValidationException()
        {
            var paymentDetails = CreatePaymentDetails(currency: "RUB");
            await Assert.ThrowsAsync<PaymentValidationException>(async () => await CreatePaymentService().ProcessPaymentAsync(paymentDetails));
        }

        [Fact]
        public async Task Given_Transaction_With_Error_Status_Assume_ProcessPaymentAsync_Returns_Payment_With_Error()
        {
            var paymentDetails = CreatePaymentDetails();
            var expectingStatus = PaymentStatus.Error;
            var bankHttpClientMock = SetupBankApiHttpClientMock(new Transaction(){ Status = (int) expectingStatus });
            var paymentService = CreatePaymentService(bankApiHttpClient: bankHttpClientMock);

            var transaction = await paymentService.ProcessPaymentAsync(paymentDetails);
            Assert.Equal(transaction.Status, expectingStatus);
        }

        [Fact]
        public async Task Unhandled_Exception_Occurs_Assume_ProcessPaymentAsync_Throws_ProcessPaymentException() 
        {
            var paymentDetails = CreatePaymentDetails();
            var paymentDetailsRepository = new Mock<IPaymentDetailsRepository>();
            paymentDetailsRepository.Setup(x => x.AddAsync(It.IsAny<Payment>())).ThrowsAsync(new Exception());
            var paymentService = CreatePaymentService(paymentDetailsRepository: paymentDetailsRepository.Object);
            
            await Assert.ThrowsAsync<ProcessPaymentException>(async () => await paymentService.ProcessPaymentAsync(paymentDetails));
        }   

        [Fact]
        public async Task Given_Card_Number_Assume_ProcessPaymentAsync_Returns_Payment_With_Masked_CardNumber()
        {
            var testCardNumber = "9872-6785-3456-2398";
            var paymentDetails = CreatePaymentDetails(cardNumber: testCardNumber);
            Payment payment = await CreatePaymentService().ProcessPaymentAsync(paymentDetails);
            
            Assert.Equal("XXXX-XXXX-XXXX-2398", payment.CardNumber);
        }

        private PaymentDetails CreatePaymentDetails(string cardNumber = "1234-1234-1234-1234", string currency = "EUR", int amount = 125, string cvv = "123", string expireAt = "12/23") =>
            new PaymentDetails() 
            { 
                CardNumber = cardNumber, 
                Amount = amount, 
                Currency = currency, 
                ExpiryAt = expireAt, 
                CVV = cvv 
            };  

        private ICurrencyRepository SetupCurrencyRepositoryMock(string currencyCode = "EUR")
        {
            var mock = new Mock<ICurrencyRepository>();
            var currency = new Currency(){ Code = currencyCode, Symbol = currencyCode };
            mock.Setup(x => x.FindBy(currencyCode)).ReturnsAsync(currency);

            return mock.Object;
        }   

        private IPaymentDetailsRepository SetupPaymentDetailsRepositoryMock()
        {
            var mock = new Mock<IPaymentDetailsRepository>();
            return mock.Object;
        }

        private IBankApiHttpClient SetupBankApiHttpClientMock(Transaction transaction = null)
        {
             if (transaction == null)
                transaction = new Transaction(){ Id = Guid.NewGuid(), Status = (int) PaymentStatus.Completed };

            var mock = new Mock<IBankApiHttpClient>();
            mock.Setup(x => x.SubmitPaymentAsync(It.IsAny<PaymentDetails>())).ReturnsAsync(transaction);

            return mock.Object;
        }

        private PaymentService CreatePaymentService(IBankApiHttpClient bankApiHttpClient = null, IPaymentDetailsRepository paymentDetailsRepository = null, ICurrencyRepository currencyRepository = null) 
        {
            if (bankApiHttpClient == null)
                bankApiHttpClient = SetupBankApiHttpClientMock();

            if (paymentDetailsRepository == null)
                paymentDetailsRepository = SetupPaymentDetailsRepositoryMock();

            if (currencyRepository == null)
                currencyRepository = SetupCurrencyRepositoryMock();

            return new PaymentService(bankApiHttpClient, paymentDetailsRepository, currencyRepository);
        }         
    }
}
