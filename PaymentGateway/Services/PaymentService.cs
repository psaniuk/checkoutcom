using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using checkoutcom.paymentgateway.Contracts;
using checkoutcom.paymentgateway.Exceptons;
using checkoutcom.paymentgateway.Models;
using checkoutcom.paymentgateway.Models.DTO;

namespace checkoutcom.paymentgateway.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly Regex _cardNumberRegex = new Regex("^(\\d{4}[- ]){3}\\d{4}|\\d{16}$"); 
        private readonly Regex _expireAtRegex = new Regex(@"^\d{1,2}\/\d{2,4}$");
        private readonly Regex _cvvRegex = new Regex(@"^(?!000)\d{3,4}$");
        private readonly IBankApiHttpClient _bankApiClient;
        private readonly IPaymentDetailsRepository _paymentDetailsRepository;
        private readonly ICurrencyRepository _currencyRepository;

        public PaymentService(IBankApiHttpClient bankApiHttpClient, IPaymentDetailsRepository paymentDetailsRepository, ICurrencyRepository currencyRepository)
        {
            _bankApiClient = bankApiHttpClient;
            _paymentDetailsRepository = paymentDetailsRepository;
            _currencyRepository = currencyRepository;
        }

        public async Task<Payment> FindAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Payment can't be empty"); 

            return await _paymentDetailsRepository.FindAsync(id);
        }

        public async Task<Payment> ProcessPaymentAsync(PaymentDetails paymentDetails)
        {
            if (paymentDetails == null)
                throw new ArgumentNullException(nameof(paymentDetails));

            try
            {
                Payment payment = await ConvertToPaymentIfValid(paymentDetails);
                Transaction transaction = await _bankApiClient.SubmitPaymentAsync(paymentDetails);
                if (transaction == null)
                    throw new ProcessPaymentException("Failed to submit a payment to a bank");
            
                if (Enum.IsDefined(typeof(PaymentStatus), transaction.Status))
                    payment.Status = (PaymentStatus)transaction.Status;
                else
                    throw new ProcessPaymentException("Payment status is unkown");

                payment.TransactionId = transaction.Id;
                await _paymentDetailsRepository.AddAsync(payment);
                
                return payment;
            }
            catch (Exception exp) when (!(exp is PaymentValidationException) && !(exp is ProcessPaymentException))
            {
                throw new ProcessPaymentException("An error has occured while processing a payment", exp);
            }
        }

        private async Task<Payment> ConvertToPaymentIfValid(PaymentDetails payment)
        {
            if (!IsCardNumberValid(payment.CardNumber))
                throw new PaymentValidationException("Card number is not valid");
            
            if (payment.Amount <= 0)
                throw new PaymentValidationException("Amount is not valid");

            if (string.IsNullOrWhiteSpace(payment.Currency))
                throw new PaymentValidationException("Currency is not valid");
            
            var currency = await _currencyRepository.FindBy(payment.Currency);
            if (currency == null)
                throw new PaymentValidationException("Currency is not valid");

            var expireAt = ParseExpireAt(payment.ExpiryAt);
            if (expireAt < DateTime.UtcNow)
                throw new PaymentValidationException("ExpireAt is not valid");
            
            if (!IsCvvNumberValid(payment.CVV))
                throw new PaymentValidationException("CVV is not valid");

            decimal amountValue = payment.Amount / 100;

            return new Payment() 
            {
                CardNumber = MaskCardNumber(payment.CardNumber),
                Amount = amountValue,
                CurrencyCode = payment.Currency,
                ExpireAt = expireAt,
                CVV = payment.CVV,
            };
        }
        
        private bool IsCardNumberValid(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return false;

            if (!_cardNumberRegex.IsMatch(cardNumber))
                return false;

            return true;
        }

        private DateTime ParseExpireAt(string expireAt)
        {
            if (string.IsNullOrWhiteSpace(expireAt))
                return DateTime.MinValue;

            if (!_expireAtRegex.IsMatch(expireAt))
                return DateTime.MinValue;

            var dateParts = expireAt.Split('/'); 
            var month = int.Parse(dateParts[0]);     
            var year = DateTime.UtcNow.Year - DateTime.UtcNow.Year % 100 + int.Parse(dateParts[1]);   

            var lastDateOfExpiryMonth = DateTime.DaysInMonth(year, month); 
            var cardExpiry = new DateTime(year, month, lastDateOfExpiryMonth, 23, 59, 59);  

            return cardExpiry;
        }

        private bool IsCvvNumberValid(string cvv)
        {
            if (string.IsNullOrWhiteSpace(cvv) || !_cvvRegex.IsMatch(cvv))
                return false;
            
            return true;
        }

        private string MaskCardNumber(string cardNumber) => $"XXXX-XXXX-XXXX-{cardNumber.Substring(cardNumber.Length - 4, 4)}";
    }
}